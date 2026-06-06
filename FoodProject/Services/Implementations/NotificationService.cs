using FoodProject.Data;
using FoodProject.Hubs;
using FoodProject.Models;
using FoodProject.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FoodProject.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(
            ApplicationDbContext context,
            IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task NotifyAdminsOrderPlacedAsync(string message, string? url = null)
        {
            var adminIds = await (
                from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id
                where r.Name == "Admin"
                select ur.UserId
            ).Distinct().ToListAsync();

            if (!adminIds.Any())
                return;

            var notifications = adminIds.Select(adminId => new Notification
            {
                AppUserId = adminId,
                Message = message,
                Url = url,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            await _hub.Clients.Users(adminIds.Select(x => x.ToString())).SendAsync("ReceiveNotification", new
            {
                message,
                url,
                createdAt = DateTime.UtcNow,
                isRead = false
            });
        }
        public async Task NotifyUserAsync(int userId, string message, string? url = null)
        {
            try
            {
                var notification = new Notification
                {
                    AppUserId = userId,
                    Message = message,
                    Url = url,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                await _hub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new
                {
                    notificationId = notification.NotificationId,
                    message = message,
                    url = url,
                    createdAt = notification.CreatedAt,
                    isRead = false
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NOTIFICATION ERROR] {ex.Message}");
            }
        }
    
    }
}
