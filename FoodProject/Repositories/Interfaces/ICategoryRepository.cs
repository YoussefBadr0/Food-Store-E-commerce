using FoodProject.Models;

namespace FoodProject.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<Category?> GetCategoryWithProductsAsync(int id);
    }
}
