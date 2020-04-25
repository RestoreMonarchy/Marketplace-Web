using Marketplace.DatabaseProvider.Repositories;
using Marketplace.DatabaseProvider.Repositories.MySql;
using Marketplace.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;

namespace Marketplace.Server.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddEconomyServices(this IServiceCollection services) 
        {
            services.AddScoped(c => new MySqlConnection(c.GetSettingValue("UconomyConnectionString")));
            services.AddTransient(c => c.GetEconomyRepository());
        }

        public static void InitializeRepositories(this IServiceScope scope)
        {
            foreach (var service in scope.ServiceProvider.GetServices<IRepository>())
            {
                service.Initialize()?.GetAwaiter().GetResult();
            }
        }

        public static IEconomyRepository GetEconomyRepository(this IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetSettingValue("EconomyProvider");
            if (provider == "AviEconomy")
                return new AviEconomyRepository(serviceProvider.GetService<MySqlConnection>());
            else
                return new UconomyEconomyRepository(serviceProvider.GetService<MySqlConnection>());
        }

        public static string GetSettingValue(this IServiceProvider serviceProvider, string settingId, bool isAdmin = true)
        {
            return serviceProvider.GetService<ISettingService>().GetSettingAsync(settingId, isAdmin).GetAwaiter().GetResult().SettingValue;
        }
    }
}