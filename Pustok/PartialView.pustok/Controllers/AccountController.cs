using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit.Text;
using MimeKit;
using PartialView.pustok.Models;
using PartialView.pustok.ViewModels;
using PartialView.pustok.ViewModels.ForgotResetPassword;
using PartialView.pustok.ViewModels.LoginingToUserPanel;
using PartialView.pustok.ViewModels.Registering;
using PartialView.pustok.ViewModels.UserAccountDetailsVM;
using PartialView.pustok.ViewModels.UserProfileViewModel;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace PartialView.pustok.Controllers
{
	public class AccountController(
		UserManager<AppUser> userManager,
		SignInManager<AppUser> signInManager,
		RoleManager<IdentityRole> roleManager) : Controller
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
			await userManager.AddToRoleAsync(user, "Member");

			return RedirectToAction("Login", "Account");
		}

		public IActionResult Login()
		{

			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginVm loginVm,string returnUrl)
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

			if (!await userManager.IsInRoleAsync(user, "Member"))
			{
				ModelState.AddModelError("", "invalid username or password");
				return View();
			}

			return returnUrl is not null? Redirect(returnUrl): RedirectToAction("Index", "Home");
		}
		[Authorize(Roles = "Member")]
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
		[Authorize(Roles = "Member")]
		public async Task<IActionResult> Profile(string tab = "dashboard")
		{ 
			ViewBag.Tab = tab;
			var user = await userManager.FindByNameAsync(User.Identity.Name);

			UserUpdateProfileVm userUpdateProfileVm = new UserUpdateProfileVm()
			{
				FullName = user.FullName,
				Email = user.Email,
				UserName = user.UserName,
			};
			UserProfileVm userProfilevm = new()
			{
				UserUpdateProfileVm = userUpdateProfileVm
			};
			return View(userProfilevm);
		}
		[Authorize(Roles = "Member")]
		[HttpPost]
		public async Task<IActionResult> Profile(UserUpdateProfileVm userUpdateProfileVm,string tab = "accountdetails")
		{
			ViewBag.Tab = tab;
			UserProfileVm userProfilevm = new()
			{
				UserUpdateProfileVm = userUpdateProfileVm
			};
			if (!ModelState.IsValid)
				return NotFound();
			var userr = await userManager.GetUserAsync(User);
			userr.FullName = userUpdateProfileVm.FullName;
			userr.Email = userUpdateProfileVm.Email;
			userr.UserName = userUpdateProfileVm.UserName;
			if (userUpdateProfileVm.NewPassword is not null)
			{
				if (userUpdateProfileVm.CurrentPassword is null)
				{
					ModelState.AddModelError("CurrentPassword", "Write previous password");
					return View(userProfilevm);
				}
				else
				{
					var newPasswordResult = await userManager.ChangePasswordAsync(userr, userUpdateProfileVm.CurrentPassword, userUpdateProfileVm.NewPassword);
					if (!newPasswordResult.Succeeded)
					{
						foreach (var error in newPasswordResult.Errors)
						{
							ModelState.AddModelError("", error.Description);
						}
						return View(userProfilevm);
					}

				}
			}
			var updateResult = await userManager.UpdateAsync(userr);
			if (!updateResult.Succeeded)
			{
				foreach (var error in updateResult.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(userProfilevm);
			}
			await signInManager.SignInAsync(userr, true);

			return RedirectToAction("Index", "Home");
		}
		public async Task<IActionResult> CreateRole()
		{
			await roleManager.CreateAsync(new IdentityRole() { Name = "Member" });
			await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
			return Content("Role Created");
		}
		public IActionResult ForgotPassword()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordVm forgotPasswordVm)
		{
			if (!ModelState.IsValid) 
				return NotFound();
			var user = await userManager.FindByEmailAsync(forgotPasswordVm.Email);
			
			if (user == null|| !await userManager.IsInRoleAsync(user,"Member"))
			{
				ModelState.AddModelError("", "Email Not Found");
				return View();
			}
			var token = await userManager.GeneratePasswordResetTokenAsync(user);
			var url = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token, },Request.Scheme);

			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("fatalizadaanar@gmail.com"));
			email.To.Add(MailboxAddress.Parse("fatalizadaanar@gmail.com"));
			email.Subject = "ResetPassword";
			using StreamReader streamReader = new StreamReader("wwwroot/templates/resetPassword.html");
			string body = await streamReader.ReadToEndAsync(); 
			body = body.Replace("{{url}}", url);
			body = body.Replace("{{username}}", user.FullName);
            email.Body = new TextPart(TextFormat.Html) { Text = body };


            using var smtp = new SmtpClient();
			smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
			smtp.Authenticate("fatalizadaanar@gmail.com", "vrea jmhh khwd wmmk");
			smtp.Send(email);
			smtp.Disconnect(true);
			return RedirectToAction("Index","Home");
		}
        public IActionResult ResetPassword()
		{
			return View();
		}
		[HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVm resetPasswordVm)
        {
			if (!ModelState.IsValid) 
				return View();
			var userr = await userManager.FindByEmailAsync(resetPasswordVm.Email.Trim());
			if (userr == null)
			{
				ModelState.AddModelError("","Email Not found");
	             return View();
			}
			var result = await userManager.ResetPasswordAsync(userr, resetPasswordVm.Token, resetPasswordVm.NewPassword);
			if (!result.Succeeded)
			{
				foreach (var err in result.Errors)
				{
					ModelState.AddModelError("",err.Description);

                }
                return View();
			}
            return RedirectToAction("Login","Account");
        }
    }
}
