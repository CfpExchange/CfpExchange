using System.ComponentModel.DataAnnotations;

namespace CfpExchange.ViewModels
{
    public class RegisterViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "E-mail address is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation must match")]
        [Required(ErrorMessage = "Password confirmation is required")]
        public string PasswordConfirmation { get; set; }
    }
}