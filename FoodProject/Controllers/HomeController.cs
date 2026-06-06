using FoodProject.Data;
using FoodProject.Models;
using FoodProject.Repositories.Interfaces;
using FoodProject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FoodProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;

        public HomeController(
            IProductRepository productRepository,
            ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.ShowSidebar = true;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CategoryDetails(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == id && x.IsActive);

            if (category == null)
                return NotFound();

            var products = await _productRepository.GetActiveProductsAsync();

            products = products
                .Where(x => x.CategoryId == id)
                .ToList();

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> About()
        {
            var about = await _context.Abouts.FirstOrDefaultAsync();
            if (about == null) return NotFound();

            return View(about);
        }

        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var products = await _productRepository.GetActiveProductsAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p =>
                    p.ProductId == id &&
                    p.Category != null &&
                    p.Category.IsActive);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(Contact model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _context.Contacts.AddAsync(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your message has been sent successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? query)
        {
            var model = new SearchViewModel { Query = query };

            if (!string.IsNullOrWhiteSpace(query))
            {
                var products = await _productRepository.GetActiveProductsAsync();

                model.Products = products
                    .Where(p => p.Name.Contains(query,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(model);
        }
    }
}