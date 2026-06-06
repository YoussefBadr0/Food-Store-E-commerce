using FoodProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodProject.ViewComponents
{
    public class ProductListViewComponent : ViewComponent
    {
        private readonly IProductRepository _productRepository;

        public ProductListViewComponent(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = await _productRepository.GetActiveProductsAsync();
            return View(products);
        }
    }
}
