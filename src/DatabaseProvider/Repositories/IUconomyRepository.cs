using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface IUconomyRepository : IRepository
    {
        Task<decimal> GetBalanceAsync(string id);
        Task<decimal> GetTotalBalanceAsync();
        Task SetBalanceAsync(string id, decimal newBalance);
    }
}
