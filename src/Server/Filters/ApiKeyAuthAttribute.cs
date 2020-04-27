using Marketplace.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Marketplace.Server.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "x-api-key";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var settingService = context.HttpContext.RequestServices.GetRequiredService<ISettingService>();
            var apiKey = await settingService.GetSettingAsync("APIKey", true);

            if (!apiKey.SettingValue.Equals(potentialApiKey))
            {
                Console.WriteLine($"Authorization failed for key {potentialApiKey}");
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
