using System.ComponentModel.DataAnnotations;

namespace FoodProject.ViewModel
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
