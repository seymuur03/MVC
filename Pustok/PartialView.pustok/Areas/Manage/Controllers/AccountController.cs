using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PartialView.pustok.Models;
using PartialView.pustok.ViewModels;

namespace PartialView.pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    //[Authorize]
    public class AccountController(UserManager<AppUser>userManager,
        SignInManager<AppUser>signInManager) : Controller
    {
        
        public async Task<IActionResult> CreateAdmin()
        {
            AppUser user = new AppUser();
            user.UserName = "admin";
            user.FullName = "admin admin";
            user.Email = "admin@gmail.com";
            var result = await userManager.CreateAsync(user,"_Admin03");
            return View(result);
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginVm adminLoginVm)
        {
            if (ModelState.IsValid) 
                return View();
            var user = await userManager.FindByNameAsync(adminLoginVm.UserName);
            if(user is null)
            {
                ModelState.AddModelError("","invalid username or password");
                return View();
            }
            var passW = await userManager.CheckPasswordAsync(user, adminLoginVm.Password);
            if (!passW)
            {
                ModelState.AddModelError("", "invalid username or password");
                return View();
            }
            await signInManager.SignInAsync(user, false); //sessiona useri atir,false = remember me

             
            //var result = await signInManager.CheckPasswordSignInAsync(user,adminLoginVm.Password,false);     // hem passwordu yoxlayir hemde sessiona add edir
            //if (!result.Succeeded)
            //{
            //    ModelState.AddModelError("", "invalid username or password");
            //    return View();
            //}
            return RedirectToAction("Index", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult UserProfile()
        {
            var user = HttpContext.User; //sistemde user varsa dolu olur 
            return Json(user.Identity);
        }
    }
}
