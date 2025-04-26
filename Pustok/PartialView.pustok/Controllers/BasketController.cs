using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using PartialView.pustok.DATA;
using PartialView.pustok.Models;
using PartialView.pustok.ViewModels.Cart;

namespace PartialView.pustok.Controllers
{
    public class BasketController(PustokDbContext pustokDbContext,
        UserManager<AppUser> userManager) : Controller
    {
        public IActionResult Index()
        {
            var basket = HttpContext.Request.Cookies["basket"];
            List<BasketVm> vm;
            if (basket != null)
            {
                vm = JsonConvert.DeserializeObject<List<BasketVm>>(basket);
            }
            else
            {
                vm = new List<BasketVm>();
                

            }

            foreach (var bo in vm)
            {
                var book = pustokDbContext.Books.Include(b => b.BookImages).FirstOrDefault(b => b.Id == bo.BookId);

                bo.MainImage = book.BookImages.FirstOrDefault(b => b.Status == true).ImgName;
                bo.Price = book.Price;
                bo.Discount = book.Discount;
                bo.ProdcutName = book.Title;


            }
            return View(vm);
        }

        public IActionResult AddTobasket(int? id)
        {
            if (id == null) 
                return NotFound();
            var eBook = pustokDbContext.Books.Include(b=>b.BookImages).FirstOrDefault(b => b.Id == id);
            if (eBook == null)
                return NotFound();
            List<BasketVm> bvm;
            var basket = HttpContext.Request.Cookies["basket"];
            if (basket != null)
                bvm = JsonConvert.DeserializeObject<List<BasketVm>>(basket);
            else
            {
                bvm = new List<BasketVm>();
               
            }

         
            if (bvm.FirstOrDefault(b=>b.BookId == id)!=null)
            {
                bvm.FirstOrDefault(b => b.BookId == id).Quantity++;
            }
            else
            {
                bvm.Add(new BasketVm
                {
                    BookId = eBook.Id,
                    Quantity = 1,
                    Price = eBook.Price,
                    ProdcutName = eBook.Title,
                    MainImage = eBook.BookImages.FirstOrDefault(bi=>bi.Status==true).ImgName,
                    Discount = eBook.Discount
                });
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = userManager.Users.Include(u => u.DbBaskets).FirstOrDefault(u=>u.UserName == User.Identity.Name);
                var UserDbBaskets = user.DbBaskets.FirstOrDefault(db => db.BookId == id);
                if (UserDbBaskets is not null)
                {
                    UserDbBaskets.Count++;
                }
                else
                {
                    user.DbBaskets.Add(new DbBasket
                    {
                        BookId = eBook.Id,
                        Count = 1,
                        AppUserId= user.Id
                    });
                }
                pustokDbContext.SaveChanges();
            }

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(bvm));

            return PartialView("_BasketPartial",bvm);
        }
    
}
}
