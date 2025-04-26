
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Data;
using System.Threading.Tasks;
using Services;
using Services.Abstractions;


using Store.API.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Shared.ErrorModels;
using Store.API.Extensions;

namespace Store.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.RegisterAllServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            await app.ConfigureMiddlewares();

            app.Run();
        }
    }
}
