using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PartialView.pustok.DATA;
using PartialView.pustok.Models;
using PartialView.pustok.Services;
using PartialView.pustok.Settings;
using PartialView.pustok.ViewModels.BookFolder;

namespace PartialView.pustok.Controllers
{
    public class BookController(PustokDbContext _pustokDbContext,
        UserManager<AppUser> userManager) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int? id)
        {
            if (id is null)
                return NotFound();
            var user = userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                var vm = GetBookDetailVm(id,user.Id);
                if (vm.Book is null)
                    return NotFound();
                return View(vm);
            }
            else
            {
                var vm = GetBookDetailVm(id);
                if (vm.Book is null)
                    return NotFound();
                return View(vm);
            }
        }

        public IActionResult BookModal(int? id)
        {
            if (id == null)
                return Content("Id cannot be null");
            var eBook = _pustokDbContext.Books.Include(b => b.Author).Include(b => b.Genre).Include(b => b.BookImages).Include(b => b.BookTags).ThenInclude(bt => bt.Tag).FirstOrDefault(b => b.Id == id);
            if (eBook == null)
                return NotFound();



            return PartialView("_ModalBookPartialView", eBook);
        }



        [Authorize(Roles = "Member")]
        [HttpPost]
        public IActionResult AddComment(BookComment bookComment)
        {
            if (!_pustokDbContext.Books.Any(b => b.Id == bookComment.BookId))
            {
                return NotFound();
            }
            var user = userManager.FindByNameAsync(User.Identity.Name).Result;

            if (user is null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Detail", "Book", new {id=bookComment.BookId}) });
            }

            if (!ModelState.IsValid)
            {
                var generalVm = GetBookDetailVm(bookComment.BookId,user.Id);
                generalVm.BookComment = bookComment;
                return View("Detail", generalVm);
            }
            bookComment.AppUserId = user.Id;
            _pustokDbContext.BookComments.Add(bookComment);
            _pustokDbContext.SaveChanges();
            return RedirectToAction("Detail", new { id = bookComment.BookId });
        }



        private BookDetailVm GetBookDetailVm(int? bookId, string userId = null)
        {
            var eBook = _pustokDbContext.Books.Include(b => b.Author).Include(b => b.Genre).Include(b => b.BookImages).Include(b => b.BookTags).ThenInclude(bt => bt.Tag).Include(b => b.BookComments).ThenInclude(bc => bc.AppUser).FirstOrDefault(b => b.Id == bookId);


            BookDetailVm bookDetailVm = new BookDetailVm()
            {
                Book = eBook,
                RelatedBooks = _pustokDbContext.Books.Include(b => b.Author).Include(b => b.Genre).Include(b => b.BookImages).Include(b => b.BookTags).ThenInclude(bt => bt.Tag).Include(b => b.BookTags).ThenInclude(bt => bt.Tag).Include(b => b.BookComments).ThenInclude(bc => bc.AppUser).Where(b => b.GenreId == eBook.GenreId || b.AuthorId == eBook.AuthorId).ToList(),

            };

            if (userId is not null)
            {
                var user = userManager.FindByNameAsync(User.Identity.Name).Result;
                bookDetailVm.HasComment = _pustokDbContext.BookComments.Any(bc => bc.AppUserId == user.Id && bc.BookId == bookId && bc.CommentStatus != CommentStatus.Rejected);

            }

            bookDetailVm.TotalComments = _pustokDbContext.BookComments.Count(bc => bc.BookId == bookId);
            bookDetailVm.AverageRate = bookDetailVm.TotalComments > 0 ? _pustokDbContext.BookComments.Where(bc => bc.BookId == bookId).Average(bc => bc.Rate) : 0;

            return bookDetailVm;
        }

        public IActionResult DeleteComment(int? id )
        {
            var user =userManager.GetUserAsync(User).Result;
            if(id is null )
                return NotFound();
            var dComment = _pustokDbContext.BookComments.Include(bc=>bc.AppUser).FirstOrDefault(bc => bc.Id == id);
            if (dComment == null)   
                return NotFound();
            if (dComment.AppUserId != user.Id)
            {
                return Content("You can not delete this comment");
            }
            _pustokDbContext.BookComments.Remove(dComment);
            _pustokDbContext.SaveChanges();

            return RedirectToAction("Detail", "Book",new { id = dComment.BookId});
        }
    }

}
