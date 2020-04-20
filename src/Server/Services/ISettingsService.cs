using Marketplace.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public interface ISettingService
    {
        Task InitializeAsync();
        ValueTask<Setting> GetSettingAsync(string settingId);
        Task UpdateSettingAsync(string settingId, string settingValue);
        ValueTask<IEnumerable<Setting>> GetSettingsAsync();
    }
}
