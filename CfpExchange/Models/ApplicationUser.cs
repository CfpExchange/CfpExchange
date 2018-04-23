using Microsoft.AspNetCore.Identity;

namespace CfpExchange.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string FullName { get; set; }
    }
}