using System.ComponentModel.DataAnnotations;

namespace FoodProject.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Food name is can't be Empty")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        public string Name { get; set; }
        [StringLength(500, ErrorMessage = "Description too long")]
        public string? Description { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Image URL is required")]
        public string ImageUrl { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Stock can't be negative")]
        public int Stock { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
