using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PartialView.pustok.DATA;
using PartialView.pustok.Models;
using PartialView.pustok.Settings;

namespace PartialView.pustok.Services
{
    public static class ServiceRegistration
    {
        public static void AllServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllersWithViews();
            services.AddDbContext<PustokDbContext>(options =>
            {
                options.UseSqlServer("Server=DESKTOP-Q4CUAVA\\SQLEXPRESS;Database=PustokDb;Trusted_Connection=True;TrustServerCertificate=True;");
            });
            services.AddScoped<LayoutService>();
            services.Configure<IOptionPatternService>(configuration.GetSection("IoptionPattern"));
            services.Configure<EmailSetting>(configuration.GetSection("Email"));
            services.AddIdentity<AppUser, IdentityRole>(option =>
            {
                option.Password.RequireLowercase = true;
                option.Password.RequireUppercase = true;
                option.Password.RequireDigit = true;
                option.Password.RequireNonAlphanumeric = true;
                option.Password.RequiredLength = 10;
                option.User.RequireUniqueEmail = true;
                option.SignIn.RequireConfirmedEmail = true;
                option.Lockout.MaxFailedAccessAttempts = 5;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            }).AddEntityFrameworkStores<PustokDbContext>().AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(option =>
            {
                option.Events.OnRedirectToLogin = option.Events.OnRedirectToAccessDenied = context =>
                {
                    var uri = new Uri(context.RedirectUri);
                    if (context.Request.Path.Value.ToLower().StartsWith("/manage"))
                    {
                        context.Response.Redirect("/manage/account/login" + uri.Query);
                    }
                    else
                    {
                        context.Response.Redirect("/account/login" + uri.Query);

                    }
                    return Task.CompletedTask;
                };
            });
            services.AddScoped<EmailService>();
            services.AddHttpContextAccessor();
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(5);
            });
        }
    }

}
