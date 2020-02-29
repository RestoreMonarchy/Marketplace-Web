using Marketplace.DatabaseProvider.Repositories;
using Marketplace.DatabaseProvider.Repositories.MySql;
using Marketplace.DatabaseProvider.Repositories.Sql;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Marketplace.DatabaseProvider.Extensions
{
    public static class DatabaseRegistratorExtensions
    {
        public static void AddMarketplaceSql(this IServiceCollection source, string connectionString)
        {
            source.AddTransient<SqlConnection>(c => new SqlConnection(connectionString));
            source.AddTransient<IMarketPlaceRepository, SqlMarketPlaceRepository>();
            source.AddTransient<IUnturnedItemAssetsRepository, SqlUnturnedItemAssetsRepository>();
            source.AddTransient<IUconomyRepository, MySqlUconomyRepository>(); //It should prob be mysql considering uconomy is always mysql?
        }

        public static void AddMarketplaceMySql(this IServiceCollection source, string connectionString)
        {
            source.AddTransient<MySqlConnection>(c => new MySqlConnection(connectionString));
            source.AddTransient<IMarketPlaceRepository, MySqlMarketPlaceRepository>();
            source.AddTransient<IUnturnedItemAssetsRepository, MySqlUnturnedItemAssetsRepository>();
            source.AddTransient<IUconomyRepository, MySqlUconomyRepository>();
        }
    }
}
