
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Data;
using System.Threading.Tasks;
using Services;
using Services.Abstractions;

using AssemblyMapper = Services.AssemblyReference;
using Store.API.MiddleWares;
using Microsoft.AspNetCore.Mvc;
using Shared.ErrorModels;

namespace Store.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StoreDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IDbInitializer, DbInitializer>(); // Allow DI for DbInitializer
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(AssemblyMapper).Assembly);
            builder.Services.AddScoped<IServiceManager, ServiceManager>();

            builder.Services.Configure<ApiBehaviorOptions>(config =>
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

            var app = builder.Build();

            #region Seeding
            using var scope = app.Services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>(); // Ask CLR to Create an Object from DbInitializer
            await dbInitializer.InitializeAsync(); // Call the InitializeAsync method to Seed the Database 
            #endregion

            app.UseMiddleware<GlobalErrorHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
