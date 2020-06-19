using Microsoft.AspNetCore.Identity;

namespace CfpExchange.Common.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}