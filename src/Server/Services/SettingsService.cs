using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Constants;
using Marketplace.Server.Settings;
using Marketplace.Shared;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public class SettingService : ISettingService
    {
        private readonly ISettingsRepository settingsRepository;
        private readonly IMemoryCache memoryCache;
        private readonly IServiceScope scope;
        private readonly ILogger<SettingService> logger;
        public SettingService(ISettingsRepository settingsRepository, IMemoryCache memoryCache, IServiceScope scope, ILogger<SettingService> logger)
        {
            this.settingsRepository = settingsRepository;
            this.memoryCache = memoryCache;
            this.scope = scope;
            this.logger = logger;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task UpdateSettingAsync(string settingId, string settingValue)
        {
            var previousValue = await GetSettingAsync(settingId, true);
            
            await settingsRepository.UpdateSettingValueAsync(settingId, settingValue);
            memoryCache.Remove(CacheKeys.SettingId(settingId));
            memoryCache.Remove(CacheKeys.SettingsId);

            foreach(var watcher in scope.ServiceProvider.GetServices<ISettingWatcher>().Where(c => c.Name == settingId))
            {

                try
                {
                    await watcher.UpdatedAsync(previousValue.SettingValue, settingValue);
                    logger.LogDebug("Executed watcher: {watcherType}, previousValue {previousValue} to new value {newValue}", watcher.GetType().Name, previousValue, settingValue);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Failed to execute watcher: {watcherType}, previousValue {previousValue} to new value {newValue}", watcher.GetType().Name, previousValue, settingValue);
                }
            }
        }

        public async ValueTask<IEnumerable<Setting>> GetSettingsAsync()
        {
            return await memoryCache.GetOrCreateAsync(CacheKeys.SettingsId, async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await settingsRepository.GetSettingsAsync();
            });
        }

        public async ValueTask<Setting> GetSettingAsync(string settingId, bool isAdmin = false)
        {
            return await memoryCache.GetOrCreateAsync(CacheKeys.SettingId(settingId), async (entry) => 
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await settingsRepository.GetSettingAsync(settingId, isAdmin);
            });
        }
    }
}
