using Dapper;
using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.Sql
{
    public class SqlSettingsRepository : ISettingsRepository
    {
        private readonly SqlConnection connection;

        public SqlSettingsRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task CreateSettingAsync(Setting setting)
        {
            const string sql = "INSERT INTO dbo.Settings (SettingId, SettingValue, Help) VALUES (@SettingId, @SettingValue, @Help);";
            await connection.ExecuteAsync(sql, setting);
        }

        public async Task<Setting> GetSettingAsync(string settingId, bool isAdmin = false)
        {
            const string sql = "SELECT * FROM dbo.Settings WHERE SettingId = @settingId AND IsAdmin = @isAdmin;";
            return (await connection.QueryAsync<Setting>(sql, new { settingId, isAdmin })).FirstOrDefault();
        }

        public async Task<IEnumerable<Setting>> GetSettingsAsync()
        {
            const string sql = "SELECT * FROM dbo.Settings;";
            return await connection.QueryAsync<Setting>(sql);
        }

        public async Task Initialize()
        {
            await AddSettingAsync(new Setting("APIKey", Guid.NewGuid().ToString("N"), "API Key for your website access", true));
            await AddSettingAsync(new Setting("UconomyConnectionString", "Server=127.0.0.1;Database=unturned;Uid=root;Password=Password123;", 
                "Connection string to uconomy database", true));
            await AddSettingAsync(new Setting("Admins", Environment.GetEnvironmentVariable("ADMIN_STEAMID"), 
                "Steam64IDs of admins seperated by comma ','"));
            await AddSettingAsync(new Setting("SteamDevKey", Environment.GetEnvironmentVariable("STEAM_DEVKEY"), "Steam API dev key", true));
            await AddSettingAsync(new Setting("IndexLayout", "Default", "Change a layout of home page"));
            await AddSettingAsync(new Setting("ItemPageLayout", "Default", "Change a layout of item page"));
            await AddSettingAsync(new Setting("TrunkLayout", "Default", "Change a layout of trunk page"));
            await AddSettingAsync(new Setting("ProductsLayout", "Default", "Change a layout of products page"));
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
