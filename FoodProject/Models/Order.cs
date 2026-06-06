using System.ComponentModel.DataAnnotations;
using FoodProject.Enums;
namespace FoodProject.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int AppUserId { get; set; }

        public AppUser AppUser { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        public string Phone { get; set; }
        [Required]
        [StringLength(200)]
        public string ShippingAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
