namespace ToDoList.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskRepository Tasks { get; }
        IApplicationUserRepository ApplicationUsers { get; }
        int Complete();
    }
}
