using Marketplace.DatabaseProvider.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Extensions
{
    public static class IUconomyRepositoryExtensions
    {
        public static async Task IncreaseBalance(this IEconomyRepository source, string id, decimal amount)
        {
            var balance = await source.GetBalanceAsync(id);
            await source.IncrementBalanceAsync(id, balance + amount);
        }
    }
}
