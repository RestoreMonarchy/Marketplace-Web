using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.Sql
{
    public class SqlProductsRepository : IProductsRepository
    {
        private readonly SqlConnection connection;
        public SqlProductsRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task GetProducts()
        {

        }
    }
}
