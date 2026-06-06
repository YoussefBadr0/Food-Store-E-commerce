using FoodProject.Data;
using FoodProject.Enums;
using FoodProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;
namespace FoodProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        public AdminOrderController(ApplicationDbContext context , INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? status = null)
        {
            var query = _context.Orders
                .Include(x => x.AppUser)
                .Include(x => x.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
                query = query.Where(x => x.Status == orderStatus);

            var orders = await query
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            ViewBag.CurrentStatus = status;
            return View(orders.ToPagedList(page, 8));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(x => x.AppUser)
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = status;
            await _context.SaveChangesAsync();

            await _notificationService.NotifyUserAsync(
                    order.AppUserId,
                    $"Your order #{order.OrderId} status changed to {order.Status}.",
                    $"/Order/Details/{order.OrderId}");

            TempData["Success"] = $"Order #{id} status updated to {status}.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.OrderId == id);

            if (order == null) return NotFound();

            _context.OrderItems.RemoveRange(order.OrderItems); 
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Order #{id} has been deleted.";
            return RedirectToAction(nameof(Index));
        }
    }

}
