using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Settings
{
    public class EconomyTypeSettingWatcher : ISettingWatcher
    {
        private readonly IServiceCollection services;

        public EconomyTypeSettingWatcher(IServiceCollection services)
        {
            this.services = services;
        }
        public string Name => "EconomyProvider";

        public Task UpdatedAsync(string previousValue, string newValue)
        {
            var descriptor = new ServiceDescriptor(typeof(IEconomyRepository), IEconomyRepositoryExtensions.ParseEconomyProviderType(newValue), ServiceLifetime.Transient);
            services.Replace(descriptor);
            return Task.CompletedTask;
        }
    }
}
