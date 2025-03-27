using Microsoft.EntityFrameworkCore;
using PartialView.pustok.DATA;
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
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

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
