using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Marketplace.Client.Providers;
using Marketplace.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using System;
using Marketplace.Client.Services;
using CurrieTechnologies.Razor.SweetAlert2;

namespace Marketplace.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, SteamAuthenticationStateProvider>();
            builder.Services.AddScoped<MarketItemsService>();
            builder.Services.AddScoped<BalanceService>();
            builder.Services.AddScoped<SettingsService>();
            builder.Services.AddScoped<PlayersService>();
            builder.Services.AddSweetAlert2(options =>
            {
                options.Theme = SweetAlertTheme.Bootstrap4;
            });

            await builder.Build().RunAsync();
        }
    }
}

