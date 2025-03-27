using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartialView.pustok.DATA;
using PartialView.pustok.ViewModels.Home;
using System.Diagnostics;

namespace PartialView.pustok.Controllers
{
    public class HomeController(PustokDbContext _pustokDbContext) : Controller
    {
        

        public IActionResult Index()
        {
            HomeVm homeVm = new HomeVm()
            {
                Sliders = _pustokDbContext.Sliders.ToList(),
                Features = _pustokDbContext.Features.ToList(),
                FeaturedBooks = _pustokDbContext.Books.Where(b=>b.IsFeatured).Include(b=>b.Author).Include(b=>b.BookImages.Where(bi=>bi.Status != null)).ToList(),
                NewBooks = _pustokDbContext.Books.Where(b=>b.IsNew).Include(b => b.Author).Include(b => b.BookImages.Where(bi => bi.Status != null)).ToList(),
                DiscBooks = _pustokDbContext.Books.Where(b=>b.Discount>0).Include(b => b.Author).Include(b => b.BookImages.Where(bi => bi.Status != null)).ToList()

            };
            return View(homeVm);
        }

    }
}
