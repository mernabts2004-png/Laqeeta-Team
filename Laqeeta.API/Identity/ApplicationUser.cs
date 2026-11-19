using Microsoft.AspNetCore.Identity;

namespace Laqeeta.API.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}