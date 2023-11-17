using FrontToBack.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(
                opt=>opt.UseSqlServer(builder.Configuration.GetConnectionString("MsSql"))
                );
            var app = builder.Build();

            app.MapControllerRoute(
                "default",
                "{controller=home}/{action=index}/{id?}"
                );

            app.UseStaticFiles();
            app.Run();
        }
    }
}