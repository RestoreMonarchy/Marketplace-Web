using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Constants;
using Marketplace.Shared;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public class SettingService : ISettingService
    {
        private readonly ISettingsRepository settingsRepository;
        private readonly IMemoryCache memoryCache;
        public SettingService(ISettingsRepository settingsRepository, IMemoryCache memoryCache)
        {
            this.settingsRepository = settingsRepository;
            this.memoryCache = memoryCache;
        }

        public string MySqlConnectionString { get; private set; }

        public string SteamDevKey { get; private set; }

        public string APIKey { get; private set; }

        public async Task InitializeAsync()
        {
            MySqlConnectionString = (await settingsRepository.GetSettingAsync("UconomyConnectionString", true)).SettingValue;
            SteamDevKey = (await settingsRepository.GetSettingAsync("SteamDevKey", true)).SettingValue;
            APIKey = (await settingsRepository.GetSettingAsync("APIKey", true)).SettingValue;
        }

        public async Task UpdateSettingAsync(string settingId, string settingValue)
        {
            await settingsRepository.UpdateSettingValueAsync(settingId, settingValue);
            memoryCache.Remove(CacheKeys.SettingId(settingId));
            await InitializeAsync();
        }

        public async Task<IEnumerable<Setting>> GetSettingsAsync()
        {
            return await settingsRepository.GetSettingsAsync();
        }

        public async Task<Setting> GetSettingAsync(string settingId)
        {
            return await memoryCache.GetOrCreateAsync(CacheKeys.SettingId(settingId), async (entry) => 
            {
                return (await settingsRepository.GetSettingAsync(settingId));
            });
        }
    }
}
