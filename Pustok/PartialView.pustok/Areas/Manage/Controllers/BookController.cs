using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PartialView.pustok.Controllers;
using PartialView.pustok.DATA;
using PartialView.pustok.Helpers;
using PartialView.pustok.Models;
using PartialView.pustok.Services;

namespace PartialView.pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly PustokDbContext _pustokDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BookController(PustokDbContext pustokDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _pustokDbContext = pustokDbContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(int page = 1, int take = 2)
        {
            var query = _pustokDbContext.Books.Include(b => b.BookImages).Include(b => b.Author).Include(b => b.Genre).AsQueryable();
            PaginatedList<Book> paginatedList = PaginatedList<Book>.PaginationMethod(query, take, page);
            return View(paginatedList);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var book = _pustokDbContext.Books.Include(b => b.BookImages).FirstOrDefault(x => x.Id == id);
            if (book == null)
                return NotFound();
            foreach (var BImage in book.BookImages)
            {
                var deleteImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/image/products", BImage.ImgName);
                FileManager.DeleteFile(deleteImagePath);
            }
            _pustokDbContext.Books.Remove(book);
            _pustokDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            ViewBag.Authors = new SelectList(_pustokDbContext.Authors.ToList(),"Id","Name");
            ViewBag.Genres = new SelectList(_pustokDbContext.Genres.ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Book book)
        {
            ViewBag.Authors = new SelectList(_pustokDbContext.Authors.ToList(), "Id", "Name");
            ViewBag.Genres = new SelectList(_pustokDbContext.Genres.ToList(), "Id", "Name");
            if (!ModelState.IsValid)
                return View();
            if (book.Photos is null )
            {
                ModelState.AddModelError("Photos", "Photos can not be empty");
                return View();
            }
            foreach (var photo in book.Photos)
            {
                BookImage bookImage = new BookImage();
                bookImage.ImgName = photo.SaveImage(_webHostEnvironment.WebRootPath, "assets/image/products");
                bookImage.BookId = book.Id;
                book.BookImages.Add(bookImage);
            }

            if (book.MainPhoto is null)
            {
                ModelState.AddModelError("MainPhoto", "MainPhoto can not be empty");
                return View();
            }
            BookImage bookMainImage = new();
            bookMainImage.Status = true;
            bookMainImage.ImgName = book.MainPhoto.SaveImage(_webHostEnvironment.WebRootPath, "assets/image/products");
            book.BookImages.Add(bookMainImage);

            if (book.HoverPhoto is null)
            {
                ModelState.AddModelError("HoverPhoto", "HoverPhoto can not be empty");
                return View();
            }
            BookImage bookHoverImage = new();
            bookHoverImage.Status = false;
            bookHoverImage.ImgName = book.HoverPhoto.SaveImage(_webHostEnvironment.WebRootPath, "assets/image/products");
            book.BookImages.Add(bookHoverImage);

            _pustokDbContext.Books.Add(book);
            _pustokDbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int?id)
        {
            ViewBag.Authors = new SelectList(_pustokDbContext.Authors.ToList(), "Id", "Name");
            ViewBag.Genres = new SelectList(_pustokDbContext.Genres.ToList(), "Id", "Name");
            if (id is null)
                return NotFound();
            var book = _pustokDbContext.Books.Include(b=>b.BookImages).Include(b => b.Author).Include(b => b.Genre).FirstOrDefault(x => x.Id == id);
            if (book == null)
                return NotFound();
            return View(book);
        }
        [HttpPost]
        public IActionResult Edit(Book book)
        {
            ViewBag.Authors = new SelectList(_pustokDbContext.Authors.ToList(), "Id", "Name");
            ViewBag.Genres = new SelectList(_pustokDbContext.Genres.ToList(), "Id", "Name");
            var Ebook = _pustokDbContext.Books.Include(b => b.BookImages).Include(b => b.Author).Include(b => b.Genre).FirstOrDefault(x => x.Id == book.Id);
            if (Ebook == null)
                return NotFound();
            if (!ModelState.IsValid)
                return View();
            
            if (book.MainPhoto is not null)
            {
                var dMainBookImage = Ebook.BookImages.FirstOrDefault(x => x.Status == true);
                _pustokDbContext.BookImages.Remove(dMainBookImage);
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets/image/products", dMainBookImage.ImgName);
                FileManager.DeleteFile(path);
                BookImage bookMainImage = new();
                bookMainImage.Status = true;
                bookMainImage.ImgName = book.MainPhoto.SaveImage(_webHostEnvironment.WebRootPath, "assets/image/products");
                Ebook.BookImages.Add(bookMainImage);
            }

            if (book.HoverPhoto is not null)
            {
                var dHoverBookImage = Ebook.BookImages.FirstOrDefault(x => x.Status==false);
                _pustokDbContext.BookImages.Remove(dHoverBookImage);
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets/image/products", dHoverBookImage.ImgName);
                FileManager.DeleteFile(path);
                BookImage bookHoverImage = new();
                bookHoverImage.Status = false;
                bookHoverImage.ImgName = book.HoverPhoto.SaveImage(_webHostEnvironment.WebRootPath, "assets/image/products");
                Ebook.BookImages.Add(bookHoverImage);
            }

            var existBookImageIds = Ebook.BookImages.Where(b=>b.Status==null).Select(b=>b.Id).ToList();
            var deleteBookImageIds = existBookImageIds.Except(book.ImgIds).ToList();
            foreach (var dImgId in deleteBookImageIds)
            {
                var dBookImage = Ebook.BookImages.FirstOrDefault(x => x.Id == dImgId);
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets/image/products", dBookImage.ImgName);
                FileManager.DeleteFile(path);
                _pustokDbContext.BookImages.Remove(dBookImage);
            }
            if (book.Photos is not null)
            {
                foreach (var photo in book.Photos)
                {
                    BookImage bookImage = new BookImage();
                    bookImage.ImgName = photo.SaveImage(_webHostEnvironment.WebRootPath, "assets/image/products");
                    bookImage.BookId = book.Id;
                    Ebook.BookImages.Add(bookImage);
                }
            }
            Ebook.Title = book.Title;
            Ebook.Price = book.Price;
            Ebook.Description = book.Description;
            Ebook.PCode = book.PCode;
            Ebook.Discount = book.Discount;
            Ebook.Rate = book.Rate;
            Ebook.AuthorId = book.AuthorId;
            Ebook.GenreId = book.GenreId;
            Ebook.IsFeatured = book.IsFeatured;
            Ebook.IsNew = book.IsNew;
            Ebook.InStock = book.InStock;
            _pustokDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Detail(int?id)
        {
            if (id == null)
                return NotFound();
            var book = _pustokDbContext.Books.Include(b=>b.BookImages).Include(b => b.Author).Include(b => b.Genre).FirstOrDefault(g => g.Id == id);
            if (book == null)
                return NotFound();
            return View(book);
        }
    }
}
