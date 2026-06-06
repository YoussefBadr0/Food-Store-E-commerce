using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodProject.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public Order? Order { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductNameSnapshot { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPriceSnapshot { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        public string? ImageSnapshot { get; set; }
        [NotMapped]
        public decimal SubTotal => Quantity * UnitPriceSnapshot;
    }
}
