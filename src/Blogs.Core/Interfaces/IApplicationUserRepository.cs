using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Core.Models.AuthModels;

namespace ToDoList.Core.Interfaces
{
    public interface IApplicationUserRepository : IBaseRepository<ApplicationUser>

    {
    }
}
