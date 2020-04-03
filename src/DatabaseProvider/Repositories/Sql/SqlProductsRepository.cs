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
            List<Product> products = new List<Product>();
            await connection.QueryAsync<Product, Server, Product>("dbo.GetProducts", 
                (p, s) => 
                {
                    var product = products.FirstOrDefault(x => x.Id == p.Id);
                    if (product == null)
                    {
                        product = p;
                        products.Add(product);                        
                        product.Servers = new List<Server>();
                    }
                    product.Servers.Add(s);
                    return p;
                }, commandType: CommandType.StoredProcedure);
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
            p.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            await connection.ExecuteAsync("dbo.CreateProduct", p, commandType: CommandType.StoredProcedure);
            return p.Get<int>("@returnValue");
        }

        public async Task UpdateProductAsync(Product product)
        {
            
        }

        public async Task DeleteProductAsync(int productId)
        {
            const string sql = "DELETE FROM dbo.Products WHERE Id = @productId;";
            await connection.ExecuteAsync(sql, new { productId });
        }
    }
}
