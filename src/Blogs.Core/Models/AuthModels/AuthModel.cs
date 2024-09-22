using System.Text.Json.Serialization;

namespace ToDoList.Core.Models.AuthModels
{
    public class AuthModel
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        //public DateTime ExpiredOn { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefrshTokenExpiration { get; set; }
    }
}
