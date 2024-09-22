using ToDoList.Core.Models.AuthModels;

namespace ToDoList.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthModel> RegisterAsync(RegisterModel model, string role = "User");
        public Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        public Task<string> AddRoleAsync(AddRoleModel model);
        public Task<AuthModel> RefreshTokenAsync(string token);
        public Task<bool> RevokeTokenAsync(string token);
    }
}

