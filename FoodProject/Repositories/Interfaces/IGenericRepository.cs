using System.Linq.Expressions;

namespace FoodProject.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<bool> ExistsAsync(int id);
        Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null);
        Task SaveAsync();
    }
}
