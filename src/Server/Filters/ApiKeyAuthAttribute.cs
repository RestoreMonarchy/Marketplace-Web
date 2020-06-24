using Marketplace.Server.Services;
using Microsoft.AspNetCore.Http;
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
            if (!await ValidateApiKeyAsync(context.HttpContext))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }

        public static async Task<bool> ValidateApiKeyAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                return false;
            }

            var settingService = context.RequestServices.GetRequiredService<ISettingService>();
            var apiKey = await settingService.GetSettingAsync("APIKey", true);

            if (apiKey?.SettingValue.Equals(potentialApiKey) ?? false)
                return true;
            else
                return false;
        }
    }
}
