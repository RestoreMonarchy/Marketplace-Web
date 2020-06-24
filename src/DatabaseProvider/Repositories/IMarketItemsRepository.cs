using Marketplace.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface IMarketItemsRepository : IRepository
    {
        Task<IEnumerable<MarketItem>> GetMarketItemsAsync();
        Task<MarketItem> GetMarketItemAsync(int id);

        Task<IEnumerable<MarketItem>> GetSellerMarketItemsAsync(string playerId);
        Task<IEnumerable<MarketItem>> GetBuyerMarketItemsAsync(string playerId);

        Task<int> SellMarketItemAsync(MarketItem marketItem);
        Task<int> BuyMarketItemAsync(int id, string buyerId);
        Task FinishBuyMarketItemAsync(int id, string buyerId, string buyerName);
        Task<int> ChangePriceMarketItemAsync(int id, string playerId, decimal price);
        Task<int> TakeDownMarketItemAsync(int id, string playerId, string playerName);
        Task ClaimMarketItemAsync(int id);
    }
}
