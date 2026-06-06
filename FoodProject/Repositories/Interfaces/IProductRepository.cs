using FoodProject.Models;

namespace FoodProject.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<Product>> GetAllWithCategoryAsync();
        Task<List<Product>> GetActiveProductsAsync();
    }
}
