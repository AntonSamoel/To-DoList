using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Core.Models.AuthModels;
using ToDoList.Services.Interfaces;

namespace ToDoList.Api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAnyOrigin")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            SetRefreshTokenInCookies(result.RefreshToken, result.RefrshTokenExpiration);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> GetTokenAsync(TokenRequestModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(requestModel);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookies(result.RefreshToken, result.RefrshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.RegisterAsync(registerModel);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookies(result.RefreshToken, result.RefrshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AddRole(AddRoleModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddRoleAsync(requestModel);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(result);
        }

        private void SetRefreshTokenInCookies(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(), // important as time in postman diffrent from local
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None

            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

        }



        [HttpPost("revoketoken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeToken revokeToken)
        {
            var refreshToken = revokeToken.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Token is required");

            var result = await _authService.RevokeTokenAsync(refreshToken);

            if (!result)
                return BadRequest("Invalid Token");

            return Ok("Revoked Successfully");
        }
    }
}
