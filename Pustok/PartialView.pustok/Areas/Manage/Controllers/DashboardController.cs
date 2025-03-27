using Microsoft.AspNetCore.Mvc;

namespace PartialView.pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class DashboardController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
