using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Core.Models.AuthModels
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        [Required, MaxLength(100)]
        public string LastName { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
