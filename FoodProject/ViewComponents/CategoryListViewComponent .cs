using FoodProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodProject.ViewComponents
{
    public class CategoryListViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryListViewComponent(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryRepository.GetAll(x => x.IsActive);
            return View(categories);
        }
    }
}