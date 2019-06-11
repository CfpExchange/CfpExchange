using System.ComponentModel.DataAnnotations;

namespace CfpExchange.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "E-mail address is required")]
        public string Email { get; set; }
    }
}
