using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Services;
using Marketplace.Server.WebSockets;
using Marketplace.Server.WebSockets.Data;
using Marketplace.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Marketplace.Server.Extensions
{
    public static class ServiceExtensions
    {
        public static void InitializeRepositories(this IServiceScope scope)
        {
            scope.ServiceProvider.GetRequiredService<ISettingsRepository>().Initialize()?.GetAwaiter().GetResult();
        }

        public static void AddWebSocketCallers(this IServiceCollection source)
        {
            source.AddSingleton<IServersService, ServerService>();
            source.AddSingleton<IWebSocketCaller>(x => x.GetRequiredService<IServersService>());
        }
        
        public static void AddWebSocketsData(this IServiceCollection source)
        {
            source.AddTransient<IEconomyWebSocketsData, EconomyWebSocketsData>();
            source.AddTransient<IProductsWebSocketsData, ProductsWebSocketsData>();
        }

        public static void InitializeWebSocketCallers(this IServiceScope scope)
        {
            var instances = new List<object>();
            foreach (var service in scope.ServiceProvider.GetServices<IWebSocketCaller>())
            {
                instances.Add(service);
            }
            scope.ServiceProvider.GetRequiredService<IWebSocketsManager>().Initialize(typeof(Program).Assembly, instances.ToArray());
        }

        public static string GetSettingValue(this IServiceProvider serviceProvider, string settingId, bool isAdmin = true)
        {
            return serviceProvider.GetRequiredService<ISettingService>().GetSettingAsync(settingId, isAdmin).GetAwaiter().GetResult().SettingValue;
        }
    }
}