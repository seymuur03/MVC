using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PartialView.pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
   // [Authorize]
    public class DashboardController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
