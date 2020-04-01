﻿using Marketplace.DatabaseProvider.Repositories;
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
            source.AddTransient<ISettingsRepository, SqlSettingsRepository>();
            source.AddTransient<IMarketItemsRepository, SqlMarketPlaceRepository>();
            source.AddTransient<IUnturnedItemsRepository, SqlUnturnedItemAssetsRepository>();
        }

        public static void AddMarketplaceMySql(this IServiceCollection source, string connectionString)
        {
            source.AddTransient<MySqlConnection>(c => new MySqlConnection(connectionString));
            source.AddTransient<IMarketItemsRepository, MySqlMarketPlaceRepository>();
            source.AddTransient<IUnturnedItemsRepository, MySqlUnturnedItemAssetsRepository>();
        }

        public static void AddUconomyMySql(this IServiceCollection source, string connectionString)
        {
            source.AddTransient<IUconomyRepository>(c => new MySqlUconomyRepository(new MySqlConnection(connectionString)));
        }
    }
}
