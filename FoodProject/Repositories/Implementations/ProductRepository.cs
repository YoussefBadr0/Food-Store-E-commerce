using FoodProject.Data;
using FoodProject.Models;
using FoodProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodProject.Repositories.Implementations
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context)
        : base(context)
        {
        }

        public async Task<List<Product>> GetAllWithCategoryAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category != null && p.Category.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category != null && p.Category.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
