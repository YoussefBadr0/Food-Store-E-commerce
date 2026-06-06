using FoodProject.Models;
using System.ComponentModel.DataAnnotations;

namespace FoodProject.ViewModel
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shipping address is required")]
        [StringLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        public List<CartItem> CartItems { get; set; } = new();
        public decimal TotalAmount => CartItems.Sum(x => x.Product.Price * x.Quantity);
    }
}
