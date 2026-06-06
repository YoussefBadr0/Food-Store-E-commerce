using System.ComponentModel.DataAnnotations;

namespace FoodProject.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string ContactName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string ContactSurname { get; set; }

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactSubject { get; set; }

        [Required]
        [StringLength(500)]
        public string ContactMessage { get; set; }
    }
}
