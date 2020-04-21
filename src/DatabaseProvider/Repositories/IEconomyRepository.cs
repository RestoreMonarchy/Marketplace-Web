using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface IEconomyRepository : IRepository
    {
        Task<decimal> GetBalanceAsync(string id);
        Task<decimal> GetTotalBalanceAsync();
        Task PayAsync(string senderId, string receiverId, decimal amount);
        Task IncrementBalanceAsync(string id, decimal amount, DateTime? date = null);
    }
}
