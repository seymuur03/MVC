using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.FileIO;
using PartialView.pustok.DATA;
using PartialView.pustok.Helpers;
using PartialView.pustok.Models;
using PartialView.pustok.Services;

namespace PartialView.pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]
    public class SliderController : Controller
    {

        private readonly PustokDbContext _pustokDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOptionPatternService _optionPatternService;
        public SliderController(PustokDbContext pustokDbContext, IWebHostEnvironment webHostEnvironment,IOptionsSnapshot<IOptionPatternService>options)
        {
            _pustokDbContext = pustokDbContext;
            _webHostEnvironment = webHostEnvironment;
            _optionPatternService = options.Value;
        }



        public IActionResult Index(int page = 1, int take = 2)
        {
            var query = _pustokDbContext.Sliders.AsQueryable();
            PaginatedList<Slider> paginatedList = PaginatedList<Slider>.PaginationMethod(query, take, page);
            return View(paginatedList);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var slider = _pustokDbContext.Sliders.FirstOrDefault(x => x.Id == id);
            if (slider == null)
                return NotFound();
            var deleteImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/image/bg-images", slider.ImgName);
            FileManager.DeleteFile(deleteImagePath);
            _pustokDbContext.Sliders.Remove(slider);
            _pustokDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slider slider)
        {
            if (slider == null)
            {
                return BadRequest("Slider object is null!");
            }
            if (!ModelState.IsValid)
                return View();
            if (slider.Photo is null)
            {
                ModelState.AddModelError("Photo", "Please, choose a photo");
                return View();
            }
            var file = slider.Photo;
            //if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
            //{
            //    ModelState.AddModelError("Photo", "File's type is not correct");
            //    return View();
            //}
            //if (file.Length > 3 * 1024 * 1024)
            //{
            //    ModelState.AddModelError("Photo", "File's length so big");
            //    return View();
            //}

            slider.ImgName = file.SaveImage(_webHostEnvironment.WebRootPath, "assets/image/bg-images");
            if (slider.ImgName == null)
            {
                ModelState.AddModelError("Photo", "Image saving failed");
                return View();
            }
            _pustokDbContext.Sliders.Add(slider);
            _pustokDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var slider = _pustokDbContext.Sliders.FirstOrDefault(g => g.Id == id);
            if (slider == null)
                return NotFound();
            return View(slider);
        }
        [HttpPost]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid)
                return View();
            var existSlider = _pustokDbContext.Sliders.FirstOrDefault(g => g.Id == slider.Id);
            if (existSlider == null)
                return NotFound();
            var file = slider.Photo;
            var oldImageName = existSlider.ImgName;
            if (file is not null)
            {
                //if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
                //{
                //    ModelState.AddModelError("Photo", "File's type is not correct");
                //    return View();
                //}
                //if (file.Length > 3 * 1024 * 1024)
                //{
                //    ModelState.AddModelError("Photo", "File's length so big");
                //    return View();
                //}
                existSlider.ImgName = file.SaveImage(_webHostEnvironment.WebRootPath, "assets/image/bg-images");
                var deleteImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/image/bg-images",oldImageName);
                FileManager.DeleteFile(deleteImagePath);    
            }
            existSlider.Name = slider.Name;
            existSlider.ButtonLink = slider.ButtonLink;
            existSlider.ButtonText = slider.ButtonText;
            existSlider.Order = slider.Order;
            existSlider.Desc = slider.Desc;
            _pustokDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int? id)
        {
            if (id == null)
                return NotFound();
            var slider = _pustokDbContext.Sliders.FirstOrDefault(g => g.Id == id);
            if (slider == null)
                return NotFound();
            return View(slider);
        }
        public IActionResult ReadData()
        {
            var d1 = _optionPatternService.Key;
            var d2 = _optionPatternService.Issuer;
            return Json(new
            {
                Key = d1,
                Issuer = d2
            });
        }
    }
}
