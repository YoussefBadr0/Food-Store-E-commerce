using System.ComponentModel.DataAnnotations;

namespace FoodProject.Models
{
    public class About
    {
        [Key]
        public int AboutId { get; set; }

        [Required]
        [StringLength(100)]
        public string AboutTitle { get; set; }

        [Required]
        [StringLength(1000)]
        public string AboutText { get; set; }

        [Required]
        [StringLength(500)]
        public string AboutImageUrl { get; set; }
    }
}
