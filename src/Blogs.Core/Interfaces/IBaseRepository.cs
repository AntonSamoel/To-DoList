using System.Linq.Expressions;

namespace ToDoList.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        T Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
