using Marketplace.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface IUnturnedItemsRepository : IRepository
    {
        Task AddUnturnedItemAsync(UnturnedItem item);
        Task SetIconAsync(int itemId, byte[] iconData);
        Task<IEnumerable<UnturnedItem>> GetUnturnedItemsAsync();
        Task<byte[]> GetItemIconAsync(int itemId);
        Task<IEnumerable<UnturnedItem>> GetUnturnedItemsIdsNoIconAsync();
        Task<UnturnedItem> GetUnturnedItemAsync(int itemId);
    }
}
