using Marketplace.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface IMarketItemsRepository : IRepository
    {
        Task<IEnumerable<MarketItem>> GetMarketItemsAsync();
        Task<MarketItem> GetMarketItemAsync(int id);
        Task<int> SellMarketItemAsync(MarketItem marketItem);
        Task<int> BuyMarketItemAsync(int id, string buyerId, decimal balance);
        Task<int> ChangePriceMarketItemAsync(int id, string playerId, decimal price);
        Task<IEnumerable<MarketItem>> GetPlayerMarketItemsAsync(string playerId);
        Task ClaimMarketItemAsync(int id);
    }
}
