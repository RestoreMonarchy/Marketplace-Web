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

        public async Task<decimal> GetProductPriceAsync(int productId)
        {
            const string sql = "SELECT Price FROM dbo.Products WHERE Id = @productId;";
            return (await connection.QueryAsync<decimal>(sql, new { productId })).FirstOrDefault();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            const string sqlAllProducts = "SELECT * FROM dbo.Products;";
            const string sqlProductServers = "SELECT s.* FROM dbo.ProductServers ps JOIN dbo.Servers s ON ps.ServerId = s.Id WHERE ps.ProductId = @Id;";
            const string sqlProductCommands = "SELECT c.* FROM dbo.ProductCommands pc JOIN dbo.Commands c ON pc.CommandId = c.Id WHERE pc.ProductId = @Id ORDER BY c.Id;";

            var products = await connection.QueryAsync<Product>(sqlAllProducts);
            foreach (var product in products)
            {
                product.Servers = await connection.QueryAsync<Server>(sqlProductServers, new { product.Id }) as List<Server>;
                product.Commands = await connection.QueryAsync<Command>(sqlProductCommands, new { product.Id }) as List<Command>;
            }

            return products;
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            var p = new DynamicParameters();
            p.Add("@Title", product.Title);
            p.Add("@Description", product.Description);
            p.Add("@Price", product.Price);
            p.Add("@Icon", product.Icon, dbType: DbType.Binary);
            p.Add("@MaxPurchases", product.MaxPurchases);
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
            p.Add("@Icon", product.Icon, dbType: DbType.Binary);
            p.Add("@MaxPurchases", product.MaxPurchases);
            p.Add("@Enabled", product.Enabled);
            p.Add("@Servers", string.Join(",", product.Servers.Select(x => x.Id)));
            p.Add("@Commands", string.Join(",", product.Commands.Select(x => x.Id)));
            await connection.ExecuteAsync("dbo.UpdateProduct", p, commandType: CommandType.StoredProcedure);            
        }

        public async Task<int> BuyProductAsync(int productId, int serverId, string buyerId)
        {
            var p = new DynamicParameters();
            p.Add("@ProductId", productId);
            p.Add("@ServerId", serverId);
            p.Add("@BuyerId", buyerId);
            p.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            await connection.ExecuteAsync("BuyProduct", p, commandType: CommandType.StoredProcedure);
            return p.Get<int>("@returnValue");
        }

        public async Task<int> FinishBuyProductAsync(int productId, int serverId, string buyerId, string buyerName)
        {
            const string sql = "INSERT INTO dbo.ProductTransactions (ProductId, ServerId, PlayerId, PlayerName) " +
                "VALUES (@productId, @serverId, @buyerId, @buyerName); SELECT SCOPE_IDENTITY();";
            return await connection.ExecuteScalarAsync<int>(sql, new { productId, serverId, buyerId, buyerName });
        }

        public async Task<IEnumerable<ProductTransaction>> GetLatestProductTransactionsAsync(int top)
        {
            return await connection.QueryAsync<ProductTransaction, Product, Server, ProductTransaction>(
                "dbo.GetLatestProductTransactions", (t, p, s) => 
                {
                    t.Product = p;
                    t.Server = s;
                    return t;
                }, new { Top = top }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ProductTransaction>> GetPlayerProductTransactionsAsync(string playerId)
        {
            return await connection.QueryAsync<ProductTransaction, Product, Server, ProductTransaction>(
                "dbo.GetPlayerProductTransactions", (t, p, s) => 
                {
                    t.Product = p;
                    t.Server = s;
                    return t;
                }, new { PlayerId = playerId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ServerTransaction>> GetServerTransactionsAsync(int serverId)
        {
            List<ServerTransaction> transactions = new List<ServerTransaction>();
            await connection.QueryAsync<ServerTransaction, ServerTransaction.Command, ServerTransaction>("dbo.GetServerProductTransactions",
              (t, c) =>
              {
                  var tran = transactions.FirstOrDefault(x => x.TransactionId == t.TransactionId);
                  if (tran == null)
                  {
                      tran = t;
                      tran.Commands = new List<ServerTransaction.Command>();
                      transactions.Add(tran);
                  }
                  tran.Commands.Add(c);
                  return null;
              }, new { serverId }, commandType: CommandType.StoredProcedure);
            return transactions;
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}
