using Domain.Interfaces;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence;
using Persistence.Identity;
using Services;
using Shared.ErrorModels;
using Store.API.Middlewares;
using System.Threading.Tasks;

namespace Store.API.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection RegisterAllServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddBuiltInServices();
            services.AddSwaggerServices();
            services.ConfigureServices();
            services.AddInfrastructureServices(configuration);
            services.AddApplicationSerivces();
            services.AddIdentityServices();

            return services;
        }

        private static IServiceCollection AddBuiltInServices(this IServiceCollection services)
        {
            services.AddControllers();

            return services;
        }

        private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        private static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<StoreIdentityDbContext>();

            return services;
        }

        private static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(config =>
            {
                config.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState.Where(m => m.Value.Errors.Any())
                                 .Select(m => new ValidationError()
                                 {
                                     Field = m.Key,
                                     Errors = m.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                 }).ToList();

                    var response = new ValidationErrorResponse(errors);

                    return new BadRequestObjectResult(response);
                };
            });

            return services; 
        }


        public static async Task<WebApplication> ConfigureMiddlewares(this WebApplication app)
        {
            await app.InitializeDatabaseAsync();

            app.UseGlobalMiddlewareHandling();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            return app;
        }

        private static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>(); // Ask CLR to Create an Object from DbInitializer
            
            await dbInitializer.InitializeAsync();
            await dbInitializer.InitializeIdentityAsync();

            return app;
        }

        private static WebApplication UseGlobalMiddlewareHandling(this WebApplication app)
        {
            app.UseMiddleware<GlobalErrorHandlingMiddleware>();

            return app;
        }
    }
}
