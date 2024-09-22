using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDoList.Core.Constants;
using ToDoList.Core.Models.AuthModels;
using ToDoList.Services.Interfaces;


namespace ToDoList.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> _userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = _userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
        }


        public async Task<AuthModel> RegisterAsync(RegisterModel model, string role = Roles.User)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered !" };
            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new AuthModel { Message = "Username is already registered !" };

            var user = new ApplicationUser
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,

            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} \n";
                }
                return new AuthModel { Message = errors };
            }
            // Create roles if they are not created
            if (!_roleManager.RoleExistsAsync(Roles.Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Roles.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.User)).GetAwaiter().GetResult();


                ApplicationUser userAdmin = new ApplicationUser()
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "admin",
                    LastName = "admin",
                    PhoneNumber = "1112223333",
                };

                await _userManager.CreateAsync(userAdmin, "Admin@2001");
                _userManager.AddToRoleAsync(userAdmin, Roles.Admin).GetAwaiter().GetResult();

            }


            await _userManager.AddToRoleAsync(user, role);

            var jwtSecurityToken = await CreateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            return new AuthModel
            {
                Email = user.Email,
                UserId = user.Id,
                UserName = user.UserName,
                IsAuthenticated = true,
                Message = "Successfully Registered",
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = refreshToken.Token,
                RefrshTokenExpiration = refreshToken.ExpiresOn,
            };

        }

        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            AuthModel authmodel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authmodel.Message = "Email or Password is incorrect !!";
                return authmodel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            //authmodel.ExpiredOn = jwtSecurityToken.ValidTo;
            authmodel.Email = user.Email;
            authmodel.UserName = user.UserName;
            authmodel.UserId = user.Id;
            authmodel.Roles = roles.ToList();
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authmodel.IsAuthenticated = true;

            if (user.RefreshTokens.Any(r => r.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(r => r.IsActive);
                authmodel.RefreshToken = activeRefreshToken.Token;
                authmodel.RefrshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authmodel.RefreshToken = refreshToken.Token;
                authmodel.RefrshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }


            return authmodel;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(_jwt.DurationInHours),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user is null || !await _roleManager.RoleExistsAsync(model.RoleName))
                return "Invalid Username or Rolename";

            if (await _userManager.IsInRoleAsync(user, model.RoleName))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            return result.Succeeded ? string.Empty : "Something went wrong";
        }

        public RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken()
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddHours(5),
                CreatedOn = DateTime.UtcNow,
            };
        }

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == token));

            if (user is null)
            {
                // Is Authenticated is false by default
                authModel.Message = "Invalid Token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(r => r.Token == token);

            if (!refreshToken.IsActive)
            {
                // Is Authenticated is false by default
                authModel.Message = "Inactive Token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();

            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefrshTokenExpiration = newRefreshToken.ExpiresOn;


            return authModel;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == token));

            if (user is null)
                return false;

            var refreshToken = user.RefreshTokens.Single(r => r.Token == token);

            if (!refreshToken.IsActive)
                return false;


            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }
    }
}
