using Admin_Dashboard.Mapping_Profiles;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.Identity;

namespace Admin_Dashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<StoreDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDbContext<StoreIdentityDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                   .AddEntityFrameworkStores<StoreIdentityDbContext>();

            builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Admin}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
