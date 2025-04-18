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

            var config = builder.Configuration;
            // Add services to the container.
            builder.Services.AllServices(config);
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
