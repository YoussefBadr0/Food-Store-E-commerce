namespace FoodProject.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyAdminsOrderPlacedAsync(string message, string? url = null);
        Task NotifyUserAsync(int userId, string message, string? url = null);
    }
}
