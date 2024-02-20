using Microsoft.AspNetCore.Identity;

namespace WebAPITrail.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name;
    }
}
