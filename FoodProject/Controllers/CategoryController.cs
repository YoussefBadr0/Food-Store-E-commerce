using FoodProject.Models;
using FoodProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var categories = string.IsNullOrWhiteSpace(search)
                ? await _categoryRepository.GetAllAsync()
                : await _categoryRepository.GetAll(x => x.Name.Contains(search));

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            category.IsActive = true;
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            var existing = await _categoryRepository.GetByIdAsync(category.CategoryId);
            if (existing == null) return NotFound();

            existing.Name = category.Name;
            existing.Description = category.Description;

            _categoryRepository.Update(existing);
            await _categoryRepository.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            category.IsActive = false;

            _categoryRepository.Update(category);

            await _categoryRepository.SaveAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}