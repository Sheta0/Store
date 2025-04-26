using Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.ErrorModels;

namespace Store.API.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
                if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    await HandlingNotFoundEndpointAsync(context);
                }
            }
            catch (Exception ex)
            {
                // log Exception
                _logger.LogError(ex, ex.Message);
                await HandlingErrorsAsync(context, ex);
            }

        }

        private static async Task HandlingErrorsAsync(HttpContext context, Exception ex)
        {
            // 1. Set Status Code of Response 
            // 2. Set Content Type of Response
            // 3. Set Response Body
            // 4. Return Response

            //context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ErrorDetails()
            {
                ErrorMessage = ex.Message
            };

            response.StatusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = response.StatusCode;

            await context.Response.WriteAsJsonAsync(response);
        }

        private static async Task HandlingNotFoundEndpointAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorDetails()
            {
                StatusCode = StatusCodes.Status404NotFound,
                ErrorMessage = $"End Point {context.Request.Path} was not found"
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
