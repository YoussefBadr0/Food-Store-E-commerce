using FoodProject.Data;
using FoodProject.Models;
using FoodProject.Repositories.Interfaces;
using FoodProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace FoodProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;

        public ProductController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository
            ,ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var products = await _productRepository.GetAllWithCategoryAsync();

            products = products
                .Where(p => p.Category != null && p.Category.IsActive)
                .ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products
                    .Where(x => x.Name.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync();
                return View(model);
            }
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                CategoryId = model.CategoryID,
                ImageUrl = await SaveImageAsync(model.Image) ?? "default.png"
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            await PopulateCategoriesAsync(product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile? Image)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync(product.CategoryId);
                return View(product);
            }

            var existingProduct = await _productRepository.GetByIdAsync(product.ProductId);
            if (existingProduct == null) return NotFound();

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.CategoryId = product.CategoryId;

            if (Image != null)
                existingProduct.ImageUrl = await SaveImageAsync(Image)
                    ?? existingProduct.ImageUrl;

            _productRepository.Update(existingProduct);
            await _productRepository.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            _productRepository.Delete(product);
            await _productRepository.SaveAsync();
            return RedirectToAction(nameof(Index));
        }


        private async Task PopulateCategoriesAsync(int selectedId = 0)
        {
            var categories = await _categoryRepository.GetAll(x => x.IsActive);
            ViewBag.Categories = categories.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CategoryId.ToString(),
                Selected = x.CategoryId == selectedId
            }).ToList();
        }

        private async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null) return null;

            var folder = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/images/products");

            Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var fullPath = Path.Combine(folder, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return fileName;
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            ViewBag.CategoryName = product.Category?.Name ?? "Uncategorized";
            return View(product);
        }
    }
}
