using Dapper;
using Marketplace.Shared;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.MySql
{
    public class MySqlSettingsRepository : ISettingsRepository
    {
        private readonly MySqlConnection connection;
        public MySqlSettingsRepository(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task CreateSettingAsync(Setting setting)
        {
            const string sql = "INSERT INTO dbo.Settings (SettingId, SettingValue, Help) VALUES (@SettingId, @SettingValue, @Help);";
            await connection.ExecuteAsync(sql, setting);
        }

        public async Task<Setting> GetSettingAsync(string settingId)
        {
            const string sql = "SELECT SettingId, SettingValue, Help FROM dbo.Settings WHERE SettingId = @settingId;";
            return (await connection.QueryAsync<Setting>(sql, new { settingId })).FirstOrDefault();
        }

        public async Task<IEnumerable<Setting>> GetSettingsAsync()
        {
            const string sql = "SELECT SettingId, SettingValue, Help FROM dbo.Settings;";
            return await connection.QueryAsync<Setting>(sql);
        }

        public async Task Initialize()
        {
            await AddSettingAsync(new Setting("IndexLayout", "Default", "Change a layout of home page"));
            await AddSettingAsync(new Setting("ItemPageLayout", "Default", "Change a layout of item page"));
        }

        private async Task AddSettingAsync(Setting setting)
        {
            await connection.ExecuteAsync("AddSetting", setting, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateSettingValueAsync(string settingId, string settingValue)
        {
            const string sql = "UPDATE dbo.Settings SET SettingValue = @settingValue WHERE SettingId = @settingId;";
            await connection.ExecuteAsync(sql, new { settingId, settingValue });
        }
    }
}
