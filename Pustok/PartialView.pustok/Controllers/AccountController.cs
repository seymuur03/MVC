using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PartialView.pustok.Models;
using PartialView.pustok.ViewModels;
using PartialView.pustok.ViewModels.LoginingToUserPanel;
using PartialView.pustok.ViewModels.Registering;

namespace PartialView.pustok.Controllers
{
	public class AccountController(
		UserManager<AppUser> userManager,
		SignInManager<AppUser> signInManager) : Controller
	{
		public IActionResult Register()
		{

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterVm registerVm)
		{
			if (!ModelState.IsValid)
				return View();
			AppUser user = await userManager.FindByNameAsync(registerVm.UserName);
			if (user is not null)
			{
				ModelState.AddModelError("UserName", "Username already used");
				return View();
			}
			user = new AppUser()
			{
				FullName = registerVm.FullName,
				Email = registerVm.Email,
				UserName = registerVm.UserName
			};

			var passW = await userManager.CreateAsync(user, registerVm.Password);
			if (!passW.Succeeded)
			{
				foreach (var passwordError in passW.Errors)
				{
					ModelState.AddModelError("", passwordError.Description);
				}
				return View();
			}

			return View();
		}

		public IActionResult Login()
		{

			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginVm loginVm)
		{
			if (!ModelState.IsValid)
				return View();

			var user = await userManager.FindByNameAsync(loginVm.UserName_Email);
			if (user is null)
			{
				user = await userManager.FindByEmailAsync(loginVm.UserName_Email);
				if (user is null)
				{
					ModelState.AddModelError("", "invalid username or password");
					return View();
				}
			}

			var result = await signInManager.PasswordSignInAsync(user, loginVm.Password, loginVm.RememberMe, true);
			if (result.IsLockedOut)
			{
				ModelState.AddModelError("", "try some time later");
				return View();
			}
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "invalid username or password");
				return View();
			}
			
			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

	}
}
