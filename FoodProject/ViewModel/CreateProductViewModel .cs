using System.ComponentModel.DataAnnotations;

namespace FoodProject.ViewModel
{
    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Food name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0.01, 99999.99, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }  

        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; set; }  

        [Range(0, 100000)]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }
    }
}
