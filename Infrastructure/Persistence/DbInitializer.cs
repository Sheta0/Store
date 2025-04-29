using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.OrderModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistence
{
    public class DbInitializer(StoreDbContext context,
                 StoreIdentityDbContext identityContext,
                 UserManager<AppUser> userManager,
                 RoleManager<IdentityRole> roleManager) : IDbInitializer
    {
        private readonly StoreDbContext _context = context;
        private readonly StoreIdentityDbContext _identityContext = identityContext;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        public async Task InitializeAsync()
        {
            // Create Database If it doesn't exist && apply to any pending Migrations
            if (_context.Database.GetPendingMigrations().Any())
                await _context.Database.MigrateAsync();

            // Data Seeding

            // Seeding ProductTypes From JSON Files
            if (!_context.ProductTypes.Any())
            {
                // 1. Read All Data from types Json file as String 
                var typesData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\types.json");

                // 2. Transform String to C# Objects [List<ProductTypes>]
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

                // 3. Add List<ProductTypes> to DbSet<ProductTypes>
                if (types is not null && types.Any())
                {
                    await _context.ProductTypes.AddRangeAsync(types);
                    await _context.SaveChangesAsync();
                }
            }



            // Seeding ProductBrands From JSON Files
            if (!_context.ProductBrands.Any())
            {
                // 1. Read All Data from types Json file as String 
                var brandsData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\brands.json");

                // 2. Transform String to C# Objects [List<ProductBrands>]
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                // 3. Add List<ProductBrands> to DbSet<ProductBrands>
                if (brands is not null && brands.Any())
                {
                    await _context.ProductBrands.AddRangeAsync(brands);
                    await _context.SaveChangesAsync();
                }
            }


            // Seeding Products From JSON Files
            if (!_context.Products.Any())
            {
                // 1. Read All Data from types Json file as String 
                var productsData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\products.json");

                // 2. Transform String to C# Objects [List<Products>]
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                // 3. Add List<Products> to DbSet<Products> 
                if (products is not null && products.Any())
                {
                    await _context.Products.AddRangeAsync(products);
                    await _context.SaveChangesAsync();
                }
            }

            // Seeding deliveryMethods From JSON Files
            if (!_context.DeliveryMethods.Any())
            {
                // 1. Read All Data from types Json file as String 
                var deliveryData = await File.ReadAllTextAsync(@"..\Infrastructure\Persistence\Data\Seeding\delivery.json");

                // 2. Transform String to C# Objects [List<DeliveryMethod>]
                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);

                // 3. Add List<deliveryMethods> to DbSet<deliveryMethods> 
                if (deliveryMethods is not null && deliveryMethods.Any())
                {
                    await _context.DeliveryMethods.AddRangeAsync(deliveryMethods);
                    await _context.SaveChangesAsync();
                }
            }

        }

        public async Task InitializeIdentityAsync()
        {
            // Create Database If it doesn't exist && apply to any pending Migrations
            if (_identityContext.Database.GetPendingMigrations().Any())
                await _identityContext.Database.MigrateAsync();

            // Seeding
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!_userManager.Users.Any())
            {
                var superAdminUser = new AppUser()
                {
                    DisplayName = "Super Admin",
                    Email = "SuperAdmin@gmail.com",
                    UserName = "SuperAdmin",
                    PhoneNumber = "0123456789"
                };

                var adminUser = new AppUser()
                {
                    DisplayName = "Admin",
                    Email = "Admin@gmail.com",
                    UserName = "Admin",
                    PhoneNumber = "0987654321"
                };

                await _userManager.CreateAsync(superAdminUser, "P@ssw0rd");
                await _userManager.CreateAsync(adminUser, "P@ssw0rd");

                await _userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
                await _userManager.AddToRoleAsync(adminUser, "Admin");

            }
        }
    }
}
