using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface IMarketPlaceRepository : IRepository
    {
        Task<IEnumerable<MarketItem>> GetMarketItemsAsync();
        Task<MarketItem> GetMarketItemAsync(int id);
        Task<int> AddMarketItemAsync(MarketItem marketItem);
        Task BuyMarketItemAsync(int id, string buyerId);
        Task ChangePriceMarketItemAsync(int id, decimal price);
        Task<IEnumerable<MarketItem>> GetPlayerMarketItemsAsync(string playerId);
        Task ClaimMarketItemAsync(int id);

    }
}
