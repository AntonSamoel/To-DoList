using ToDoList.Core.Interfaces;
using ToDoList.DataAccess.Data;

namespace ToDoList.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ITaskRepository Tasks { get; private set; }
        public IApplicationUserRepository ApplicationUsers { get; private set; }


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Tasks = new TaskRepository(context);
            ApplicationUsers = new ApplicationUserRepository(context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
