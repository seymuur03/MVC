using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartialView.pustok.DATA;
using PartialView.pustok.Helpers;
using PartialView.pustok.Models;

namespace PartialView.pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    //[Authorize]
    public class GenreController(PustokDbContext _pustokDbContext) : Controller
    {
        public IActionResult Index(int page=1,int take=2)
        {
            var query = _pustokDbContext.Genres.AsQueryable();
            PaginatedList<Genre>paginatedList=PaginatedList<Genre>.PaginationMethod(query, take,page);
            return View(paginatedList);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var genre = _pustokDbContext.Genres.FirstOrDefault(x => x.Id == id);
            if(genre==null)
                return NotFound();
            _pustokDbContext.Genres.Remove(genre);
            _pustokDbContext.SaveChanges(); 
            return Ok();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            if (!ModelState.IsValid) 
                return View();
            var existg= _pustokDbContext.Genres.FirstOrDefault(eg=>eg.Name.ToUpper()==genre.Name.ToUpper());
            if (existg is not null)
            {
                ModelState.AddModelError("Name", "There is a genre named like that");
                return View();
            }
            _pustokDbContext.Genres.Add(genre);
            _pustokDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var genre = _pustokDbContext.Genres.FirstOrDefault(g=>g.Id==id);
            if (genre == null)
                return NotFound();
            return View(genre);
        }
        [HttpPost]
        public IActionResult Edit(Genre genre)
        {
            if (!ModelState.IsValid)
                return View();
            var existG = _pustokDbContext.Genres.FirstOrDefault(eg=>eg.Id==genre.Id);
            if (existG is null) 
                return NotFound();
            var existg = _pustokDbContext.Genres.FirstOrDefault(eg => eg.Name.ToUpper() == genre.Name.ToUpper()&&eg.Id!=genre.Id);
            if (existg is not null)
            {
                ModelState.AddModelError("Name", "There is a genre named like that");
                return View();
            }
            existG.Name = genre.Name;
            _pustokDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Detail(int? id)
        {
            if (id == null)
                return NotFound();
            var genre = _pustokDbContext.Genres.Include(g=>g.Books).FirstOrDefault(g => g.Id == id);
            if (genre == null)
                return NotFound();
            return View(genre);
        }
    }
}
