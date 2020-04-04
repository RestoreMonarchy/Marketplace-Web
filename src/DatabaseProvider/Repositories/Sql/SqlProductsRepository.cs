using Dapper;
using Marketplace.Shared;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            const string sqlAllProducts = "SELECT * FROM dbo.Products;";
            const string sqlProductServers = "SELECT s.* FROM dbo.ProductServers ps JOIN dbo.Servers s ON ps.ServerId = s.Id WHERE ps.ProductId = @Id;";
            const string sqlProductCommands = "SELECT c.* FROM dbo.ProductCommands pc JOIN dbo.Commands c ON pc.CommandId = c.Id WHERE pc.ProductId = @Id ORDER BY c.Id;";

            var products = await connection.QueryAsync<Product>(sqlAllProducts);
            foreach (var product in products)
            {
                product.Servers = await connection.QueryAsync<Server>(sqlProductServers, new { product.Id });
                product.Commands = await connection.QueryAsync<Command>(sqlProductCommands, new { product.Id });
            }

            return products;
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            var p = new DynamicParameters();
            p.Add("@Title", product.Title);
            p.Add("@Description", product.Description);
            p.Add("@Price", product.Price);
            p.Add("@ExecuteCommands", product.ExecuteCommands);
            p.Add("@Icon", product.Icon);
            p.Add("@Expires", product.Expires);
            p.Add("@Enabled", product.Enabled);
            p.Add("@Servers", string.Join(",", product.Servers.Select(x => x.Id)));
            p.Add("@Commands", string.Join(",", product.Commands.Select(x => x.Id)));
            p.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            await connection.ExecuteAsync("dbo.CreateProduct", p, commandType: CommandType.StoredProcedure);
            return p.Get<int>("@returnValue");
        }

        public async Task UpdateProductAsync(Product product)
        {
            var p = new DynamicParameters();
            p.Add("@Id", product.Id);
            p.Add("@Title", product.Title);
            p.Add("@Description", product.Description);
            p.Add("@Price", product.Price);
            p.Add("@ExecuteCommands", product.ExecuteCommands);
            p.Add("@Icon", product.Icon);
            p.Add("@Expires", product.Expires);
            p.Add("@Enabled", product.Enabled);
            p.Add("@Servers", string.Join(",", product.Servers.Select(x => x.Id)));
            p.Add("@Commands", string.Join(",", product.Commands.Select(x => x.Id)));
            await connection.ExecuteAsync("dbo.CreateProduct", p, commandType: CommandType.StoredProcedure);            
        }
    }
}
