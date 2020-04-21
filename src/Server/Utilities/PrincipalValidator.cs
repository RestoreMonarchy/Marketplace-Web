using Marketplace.Server.Services;
using Marketplace.Shared.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Marketplace.Server.Utilities
{
    public static class PrincipalValidator
    {
        public static async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            string steamId = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value.Substring(37);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, steamId));

            var settingService = context.HttpContext.RequestServices.GetRequiredService<ISettingService>();
            var admins = (await settingService.GetSettingAsync("Admins")).SettingValue.Split(',');
            
            if (Environment.GetEnvironmentVariable("ADMIN_STEAMID") == steamId || admins.Contains(steamId))
                claims.Add(new Claim(ClaimTypes.Role, RoleConstants.AdminRoleId));
            else
                claims.Add(new Claim(ClaimTypes.Role, RoleConstants.GuestRoleId));

            context.ReplacePrincipal(new ClaimsPrincipal(new ClaimsIdentity(claims, "DefaultAuth")));
        }
    }
}
