using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PartialView.pustok.Models;
using PartialView.pustok.ViewModels;

namespace PartialView.pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles ="Admin")]
    public class AccountController(UserManager<AppUser>userManager,
        SignInManager<AppUser>signInManager) : Controller
    {
        
        public async Task<IActionResult> CreateAdmin()
        {
            AppUser user = new AppUser();
            user.UserName = "admin";
            user.FullName = "admin admin";
            user.Email = "admin@gmail.com";
            var result = await userManager.CreateAsync(user,"_Admin1903");
            if (result.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(user, "Admin");
            }
            return Json(result);
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginVm adminLoginVm,string returnUrl)
        {
            if (!ModelState.IsValid) 
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

            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError("", "invalid username or password");
                return View();
            }



            await signInManager.SignInAsync(user, false); //sessiona useri atir,false = remember me


            //var result = await signInManager.CheckPasswordSignInAsync(user, adminLoginVm.Password, false);     // hem passwordu yoxlayir hemde sessiona add edir
            //if (!result.Succeeded)
            //{
            //    ModelState.AddModelError("", "invalid username or password");
            //    return View();
            //}
            return returnUrl is not null ? Redirect(returnUrl) : RedirectToAction("Index", "Dashboard");
        }
        
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account","Manage");
        }
		public IActionResult UserProfile()
        {
            var user = HttpContext.User; //sistemde user varsa dolu olur 
            return Json(user.Identity);
        }
    }
}
