using Marketplace.DatabaseProvider.Repositories;
using Marketplace.DatabaseProvider.Repositories.MySql;
using Marketplace.DatabaseProvider.Repositories.Sql;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace Marketplace.DatabaseProvider.Extensions
{
    public static class DatabaseRegistratorExtensions
    {
        public static void AddMarketplaceSql(this IServiceCollection source, string connectionString)
        {
            source.AddTransient<SqlConnection>(c => new SqlConnection(connectionString));
            source.AddTransient<ISettingsRepository, SqlSettingsRepository>();
            source.AddTransient<IMarketItemsRepository, SqlMarketItemsRepository>();
            source.AddTransient<IUnturnedItemsRepository, SqlUnturnedItemsRepository>();
            source.AddTransient<IServersRepository, SqlServersRepository>();
            source.AddTransient<IProductsRepository, SqlProductsRepository>();
            source.AddTransient<ICommandsRepository, SqlCommandsRepository>();
        }
    }
}
