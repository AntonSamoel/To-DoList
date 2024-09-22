using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Core.Interfaces;
using ToDoList.DataAccess.Data;
using M = ToDoList.Core.Models;

namespace ToDoList.DataAccess.Repositories
{
    public class TaskRepository : BaseRepository<M.Task> ,ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
