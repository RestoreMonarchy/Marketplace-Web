using Marketplace.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public interface ISettingService
    {
        string MySqlConnectionString { get; }
        string SteamDevKey { get; }
        string APIKey { get; }

        Task InitializeAsync();
        Task<Setting> GetSettingAsync(string settingId);
        Task UpdateSettingAsync(string settingId, string settingValue);
        Task<IEnumerable<Setting>> GetSettingsAsync();
    }
}
