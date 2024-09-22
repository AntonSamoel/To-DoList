using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Core.Interfaces;
using ToDoList.Core.Models.AuthModels;
using ToDoList.DataAccess.Data;

namespace ToDoList.DataAccess.Repositories
{
    public class ApplicationUserRepository:BaseRepository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
