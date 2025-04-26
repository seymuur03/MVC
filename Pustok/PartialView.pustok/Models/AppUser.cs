using Microsoft.AspNetCore.Identity;
using PartialView.pustok.ViewModels.Cart;

namespace PartialView.pustok.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
        public List<DbBasket> DbBaskets { get; set; }
    }
}
