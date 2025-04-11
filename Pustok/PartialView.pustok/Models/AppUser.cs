using Microsoft.AspNetCore.Identity;

namespace PartialView.pustok.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
