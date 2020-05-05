﻿using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Client.Providers;
using Marketplace.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, SteamAuthenticationStateProvider>();
            services.AddScoped<OrderState>();
            services.AddScoped<BalanceService>();

            services.AddSweetAlert2(options =>
            {
                options.Theme = SweetAlertTheme.Bootstrap4;
            });
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
