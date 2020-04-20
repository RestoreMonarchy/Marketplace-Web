using Marketplace.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface ISettingsRepository : IRepository
    {
        Task<Setting> GetSettingAsync(string settingId, bool isAdmin = false);
        Task<IEnumerable<Setting>> GetSettingsAsync();
        Task CreateSettingAsync(Setting setting);
        Task UpdateSettingValueAsync(string settingId, string settingValue);        
    }
}
