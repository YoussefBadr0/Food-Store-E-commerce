using FoodProject.Data;
using FoodProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodProject.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartItems = await _context.CartItems
                .Include(x => x.Product)
                .Where(x => x.AppUserId == GetUserId())
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = GetUserId();

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var existing = await _context.CartItems
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.AppUserId == userId);

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    AppUserId = userId,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"{product.Name} added to cart!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item == null || item.AppUserId != GetUserId()) return NotFound();

            if (quantity <= 0)
                _context.CartItems.Remove(item);
            else
                item.Quantity = quantity;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item == null || item.AppUserId != GetUserId()) return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Item removed from cart.";
            return RedirectToAction(nameof(Index));
        }
    }
}