using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PartialView.pustok.DATA;
using PartialView.pustok.Models;
using PartialView.pustok.ViewModels.Cart;

namespace PartialView.pustok.Services
{
    public class LayoutService(PustokDbContext _pustokDbContext,
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager)
    {
        public List<Setting> GetSettings()
        {
            return _pustokDbContext.Settings.ToList();
        }

        public List<Genre> GetGenres()
        {
            return _pustokDbContext.Genres.ToList();
        }
        public List<BasketVm> GetBasketItems()
        {
            var httpcontex = httpContextAccessor.HttpContext;
            var basket = httpcontex.Request.Cookies["basket"];
            var list = new List<BasketVm>();
            if (basket is  not null) 
                list =  JsonConvert.DeserializeObject<List<BasketVm>>(basket);


            foreach (var bo in list)
            {
                var book = _pustokDbContext.Books.Include(b => b.BookImages).FirstOrDefault(b => b.Id == bo.BookId);

                bo.MainImage = book.BookImages.FirstOrDefault(b => b.Status == true).ImgName;
                bo.Price = book.Price;
                bo.Discount = book.Discount;
                bo.ProdcutName = book.Title;


            }
            if (httpcontex.User.Identity.IsAuthenticated)
            {
                var user = userManager.Users.Include(u=>u.DbBaskets).ThenInclude(dbb=>dbb.Book).ThenInclude(dbb=>dbb.BookImages).FirstOrDefault(u=>u.UserName == httpcontex.User.Identity.Name);
              foreach (var uDb in user.DbBaskets)
                {
                    if (!list.Any(l=>l.BookId==uDb.BookId))
                    {
                        list.Add(new BasketVm
                        {
                            BookId = uDb.BookId,
                            ProdcutName = uDb.Book.Title,
                            MainImage = uDb.Book.BookImages.FirstOrDefault(bi=>bi.Status==true).ImgName,
                            Price = uDb.Book.Price,
                            Quantity = uDb.Count 
                        });
                    }
                }
                httpcontex.Response.Cookies.Append("basket",JsonConvert.SerializeObject(list));
            }
            return list;

        }

    }
}
