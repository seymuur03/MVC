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
using PartialView.pustok.ViewModels.ConfirmEmail;
using PartialView.pustok.Services;
using Microsoft.CodeAnalysis.Options;
using PartialView.pustok.Settings;
using Microsoft.Extensions.Options;

namespace PartialView.pustok.Controllers
{
	public class AccountController(
		UserManager<AppUser> userManager,
		SignInManager<AppUser> signInManager,
		RoleManager<IdentityRole> roleManager,
		EmailService emailService,
		IOptions<EmailSetting> emailSetting) : Controller
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

			if (registerVm.Password.ToLower() == registerVm.UserName.ToLower())
			{
                ModelState.AddModelError("", "Password must be differ from username");
                return View();
            }

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

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action("VerificationEmail", "Account", new { email = user.Email, token = token, }, Request.Scheme);

            using StreamReader streamReader = new StreamReader("wwwroot/templates/VerifyEmail.html");
            string body = await streamReader.ReadToEndAsync();
            body = body.Replace("{{url}}", url);
            body = body.Replace("{{username}}", user.FullName);

            emailService.SendEmail(user.Email, "ConfirmAccount", body, "fatalizadaanar@gmail.com", "vrea jmhh khwd wmmk",emailSetting.Value);

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

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Confirm email adress");
                return View();
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

			
			using StreamReader streamReader = new StreamReader("wwwroot/templates/resetPassword.html");
			string body = await streamReader.ReadToEndAsync(); 
			body = body.Replace("{{url}}", url);
			body = body.Replace("{{username}}", user.FullName);
            emailService.SendEmail(user.Email, "ResetPassword", body, "fatalizadaanar@gmail.com", "vrea jmhh khwd wmmk",emailSetting.Value);

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

		public async Task<IActionResult> VerificationEmail(VerifyEmailVm verifyEmailVm)
		{
			if(verifyEmailVm.Email is null || verifyEmailVm.Token is null)
				return NotFound();
			var user = await userManager.FindByEmailAsync(verifyEmailVm.Email);
			if (user == null)
				return NotFound();
			var resultConfirm = userManager.ConfirmEmailAsync(user,token:verifyEmailVm.Token).Result;
			if (!resultConfirm.Succeeded)
				return NotFound();
			return RedirectToAction("Login","Account");
		}
    }
}
