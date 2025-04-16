using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PartialView.pustok.DATA;
using PartialView.pustok.Models;
using PartialView.pustok.Services;

namespace PartialView.pustok
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<PustokDbContext>(options =>
            {
                options.UseSqlServer("Server=DESKTOP-Q4CUAVA\\SQLEXPRESS;Database=PustokDb;Trusted_Connection=True;TrustServerCertificate=True;");
            });
            builder.Services.AddScoped<LayoutService>();
            builder.Services.Configure<IOptionPatternService>(builder.Configuration.GetSection("IoptionPattern"));
            builder.Services.AddIdentity<AppUser, IdentityRole>(option =>
            {
                option.Password.RequireLowercase = true;
                option.Password.RequireUppercase = true;
                option.Password.RequireDigit = true;
                option.Password.RequireNonAlphanumeric = true;
                option.Password.RequiredLength = 10;
                option.User.RequireUniqueEmail = true;
                option.Lockout.MaxFailedAccessAttempts = 5;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            }).AddEntityFrameworkStores<PustokDbContext>().AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(option =>
            {
                option.Events.OnRedirectToLogin = option.Events.OnRedirectToAccessDenied = context =>
                {
                    var uri = new Uri(context.RedirectUri);
                    if(context.Request.Path.Value.ToLower().StartsWith("/manage"))
                    {
                        context.Response.Redirect("/manage/account/login"+uri.Query);
                    }
                    else
                    {
						context.Response.Redirect("/account/login" + uri.Query);

					}
                    return Task.CompletedTask;
				};
            });
            var app = builder.Build();


            // Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); //user sistemini yoxlayir
            app.UseAuthorization(); // role ucundu

            app.MapControllerRoute(
              name: "areas",
              pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
