using FoodProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodProject.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var notifications = await _context.Notifications
                .Where(x => x.AppUserId == GetUserId())
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == id && x.AppUserId == userId);

            if (notification == null) return NotFound();

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(notification);
        }

        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            var count = await _context.Notifications
                .CountAsync(x => x.AppUserId == GetUserId() && !x.IsRead);

            return Json(count);
        }

        [HttpGet]
        public async Task<IActionResult> Latest()
        {
            var notifications = await _context.Notifications
                .Where(x => x.AppUserId == GetUserId())
                .OrderByDescending(x => x.CreatedAt)
                .Take(5)
                .Select(x => new
                {
                    x.NotificationId,
                    x.Message,
                    x.Url,
                    x.CreatedAt,
                    x.IsRead
                })
                .ToListAsync();

            return Json(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetUserId();
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == id && x.AppUserId == userId);

            if (notification == null) return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}