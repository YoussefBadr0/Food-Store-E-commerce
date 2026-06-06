using System.ComponentModel.DataAnnotations;

namespace FoodProject.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        [Range(1, 100)]
        public int Quantity { get; set; } = 1;
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
