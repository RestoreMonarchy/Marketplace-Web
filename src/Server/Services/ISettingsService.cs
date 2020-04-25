using Marketplace.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public interface ISettingService
    {
        ValueTask<Setting> GetSettingAsync(string settingId, bool isAdmin = false);
        Task UpdateSettingAsync(string settingId, string settingValue);
        ValueTask<IEnumerable<Setting>> GetSettingsAsync();
    }
}
