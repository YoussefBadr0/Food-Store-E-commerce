using System.ComponentModel.DataAnnotations;

namespace FoodProject.ViewModel
{
    public class AboutUpdateViewModel
    {
        public int AboutId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3)]  
        public string AboutTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Text is required")]
        [StringLength(1000, MinimumLength = 10)]  
        public string AboutText { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; }       
        public string? CurrentImageUrl { get; set; }
    }
}
