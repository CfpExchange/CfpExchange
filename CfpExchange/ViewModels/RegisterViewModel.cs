using System.ComponentModel.DataAnnotations;

namespace CfpExchange.ViewModels
{
    public class RegisterViewModel
    {
        [EmailAddress]
        public string EmailAddress { get; set; }
        
        [Required]
        public string FullName { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }
    }
}