using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M =ToDoList.Core.Models;

namespace ToDoList.Core.Interfaces
{
    public interface ITaskRepository : IBaseRepository<M.Task>
    {
    }
}
