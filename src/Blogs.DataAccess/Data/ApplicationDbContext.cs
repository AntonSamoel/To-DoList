using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoList.Core.Models.AuthModels;
using  M =  ToDoList.Core.Models;



namespace ToDoList.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

       
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<M.Task> Tasks { get; set; }
    }
}
