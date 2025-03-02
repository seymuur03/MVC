using Microsoft.AspNetCore.Mvc;
using PartialView.pustok.DATA;
using PartialView.pustok.Models;
using System.Diagnostics;

namespace PartialView.pustok.Controllers
{
    public class HomeController(PustokDbContext _pustokDbContext) : Controller
    {
        

        public IActionResult Index()
        {
            return View();
        }

    }
}
