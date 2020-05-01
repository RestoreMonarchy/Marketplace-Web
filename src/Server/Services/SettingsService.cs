﻿using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Constants;
using Marketplace.Shared;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public class SettingService : ISettingService
    {
        private readonly ISettingsRepository settingsRepository;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<ISettingService> logger;
        public SettingService(ISettingsRepository settingsRepository, IMemoryCache memoryCache, ILogger<ISettingService> logger)
        {
            this.settingsRepository = settingsRepository;
            this.memoryCache = memoryCache;
            this.logger = logger;
        }

        public async Task UpdateSettingAsync(string settingId, string settingValue)
        {            
            await settingsRepository.UpdateSettingValueAsync(settingId, settingValue);
            memoryCache.Remove(CacheKeys.SettingId(settingId));
            memoryCache.Remove(CacheKeys.SettingsId);
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
