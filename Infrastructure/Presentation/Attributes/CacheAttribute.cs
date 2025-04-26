using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Attributes
{
    public class CacheAttribute(int durationInSec) : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cahceService = context.HttpContext.RequestServices.GetRequiredService<IServiceManager>().CacheService;

            var cacheKey = GenerateCacheKey(context.HttpContext.Request);
            var result = await cahceService.GetCacheValueAsync(cacheKey);
            if (!string.IsNullOrEmpty(result))
            {
                // Return Response
                context.Result = new ContentResult()
                {
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK,
                    Content = result
                };
                return;
            }
            // Execute the endpoint
            var contextResult = await next();
            if (contextResult.Result is OkObjectResult ok)
            {
                await cahceService.SetCacheValueAsync(cacheKey, ok.Value, TimeSpan.FromSeconds(durationInSec));
            }
        }

        private string GenerateCacheKey(HttpRequest request)
        {
            var key = new StringBuilder();
            key.Append(request.Path);
            foreach (var query in request.Query.OrderBy(q => q.Key))
            {
                key.Append($"|{query.Key}-{query.Value}");
            }

            return key.ToString();
        }
    }
}
