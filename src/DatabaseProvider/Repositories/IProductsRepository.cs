using Marketplace.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface IProductsRepository : IRepository
    {
        Task<decimal> GetProductPriceAsync(int productId);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<int> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task<int> BuyProductAsync(int productId, int serverId, string buyerId);
        Task<int> FinishBuyProductAsync(int productId, int serverId, string buyerId, string buyerName);
        Task<IEnumerable<ProductTransaction>> GetLatestProductTransactionsAsync(int top);
        Task<IEnumerable<ProductTransaction>> GetPlayerProductTransactionsAsync(string playerId);
        Task<IEnumerable<ServerTransaction>> GetServerTransactionsAsync(int serverId);
    }
}
