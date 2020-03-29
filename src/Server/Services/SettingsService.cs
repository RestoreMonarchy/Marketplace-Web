using Marketplace.DatabaseProvider.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public class SettingsService
    {
        private ISettingsRepository settingsRepository;
        public SettingsService(ISettingsRepository settingsRepository)
        {
            this.settingsRepository = settingsRepository;
        }

        public async Task<int> GetIntValueAsync(string key)
        {            
            return Convert.ToInt32((await settingsRepository.GetSettingAsync(key)).SettingValue);
        }

        public async Task<bool> GetBoolValueAsync(string key)
        {
            return Convert.ToBoolean((await settingsRepository.GetSettingAsync(key)).SettingValue);
        }

        public async Task<string> GetValueAsync(string key)
        {
            return (await settingsRepository.GetSettingAsync(key)).SettingValue;
        }
    }
}
