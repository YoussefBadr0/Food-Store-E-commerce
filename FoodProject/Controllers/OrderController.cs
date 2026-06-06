using FoodProject.Data;
using FoodProject.Enums;
using FoodProject.Models;
using FoodProject.Services.Interfaces;
using FoodProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodProject.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        public OrderController(ApplicationDbContext context , INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = GetUserId();
            var cartItems = await _context.CartItems
                .Include(x => x.Product)
                .Where(x => x.AppUserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel { CartItems = cartItems };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = GetUserId();

            model.CartItems = await _context.CartItems
                .Include(x => x.Product)
                .Where(x => x.AppUserId == userId)
                .ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            if (!model.CartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                AppUserId = userId,
                OrderDate = DateTime.UtcNow,
                Phone = model.Phone,
                ShippingAddress = model.ShippingAddress,
                City = model.City,
                Status = OrderStatus.Pending,
                TotalAmount = model.CartItems.Sum(x => x.Product.Price * x.Quantity),
                OrderItems = model.CartItems.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    ProductNameSnapshot = x.Product.Name,
                    UnitPriceSnapshot = x.Product.Price,
                    Quantity = x.Quantity,
                    ImageSnapshot = x.Product.ImageUrl
                }).ToList()
            };

            _context.Orders.Add(order);

            _context.CartItems.RemoveRange(model.CartItems);

            await _context.SaveChangesAsync();

            await _notificationService.NotifyAdminsOrderPlacedAsync(
                $"New order #{order.OrderId} has been placed.",
                $"/AdminOrder/Details/{order.OrderId}");

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction(nameof(Details), new { id = order.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = GetUserId();
            var orders = await _context.Orders
                .Include(x => x.OrderItems)
                .Where(x => x.AppUserId == userId)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();
            var order = await _context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.OrderId == id && x.AppUserId == userId);

            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetUserId();
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.OrderId == id && x.AppUserId == userId);

            if (order == null) return NotFound();

            if (order.Status != OrderStatus.Pending)
            {
                TempData["Error"] = "Only pending orders can be cancelled.";
                return RedirectToAction(nameof(Details), new { id });
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Order cancelled successfully.";
            return RedirectToAction(nameof(MyOrders));
        }
    }
}
