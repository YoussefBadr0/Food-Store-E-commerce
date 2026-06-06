namespace FoodProject.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;

        public string Message { get; set; } = string.Empty;

        public string? Url { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
