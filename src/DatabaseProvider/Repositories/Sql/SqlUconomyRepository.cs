using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.Sql
{
    public sealed class SqlUconomyRepository : IUconomyRepository
    {
        private readonly SqlConnection connection;

        public SqlUconomyRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

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
