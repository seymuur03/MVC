using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PartialView.pustok.DATA;
using PartialView.pustok.Services;
using PartialView.pustok.Settings;
using PartialView.pustok.ViewModels.BookFolder;

namespace PartialView.pustok.Controllers
{
    public class BookController(PustokDbContext _pustokDbContext) : Controller
    {
        public IActionResult Index()
        { 
            return View();
        }

        public IActionResult Detail(int? id)
        {
            if(id == null)
                return Content("Id cannot be null");
            var eBook=_pustokDbContext.Books.Include(b=>b.Author).Include(b=>b.Genre).Include(b=>b.BookImages).Include(b=>b.BookTags).ThenInclude(bt=>bt.Tag).FirstOrDefault(b=>b.Id==id);
            if(eBook == null) 
                return NotFound();

            BookDetailVm bookDetailVm = new BookDetailVm()
            {
                Book = eBook,
                RelatedBooks = _pustokDbContext.Books.Include(b => b.Author).Include(b => b.Genre).Include(b => b.BookImages).Include(b => b.BookTags).ThenInclude(bt => bt.Tag).Where(b => b.GenreId == eBook.GenreId || b.AuthorId == eBook.AuthorId).ToList(),
            };

            return View(bookDetailVm);
        }

        public IActionResult BookModal(int? id)
        {
            if (id == null)
                return Content("Id cannot be null");
            var eBook = _pustokDbContext.Books.Include(b => b.Author).Include(b => b.Genre).Include(b => b.BookImages).Include(b => b.BookTags).ThenInclude(bt => bt.Tag).FirstOrDefault(b => b.Id == id);
            if (eBook == null)
                return NotFound();

         

            return PartialView("_ModalBookPartialView",eBook);
        }

    }
}
