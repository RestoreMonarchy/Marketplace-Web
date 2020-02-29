using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.MySql
{
    public sealed class MySqlUconomyRepository : IUconomyRepository
    {
        public Task<decimal> GetBalanceAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task SetBalanceAsync(string id, decimal newBalance)
        {
            throw new NotImplementedException();
        }
    }
}
