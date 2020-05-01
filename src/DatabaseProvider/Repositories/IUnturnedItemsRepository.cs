using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface IUnturnedItemsRepository : IRepository
    {
        Task AddUnturnedItemAsync(UnturnedItem item);
        Task SetIconAsync(ushort itemId, byte[] iconData);
        Task<IEnumerable<UnturnedItem>> GetUnturnedItemsAsync();
        Task<Stream> GetItemIconAsync(ushort itemId);
        Task<IEnumerable<UnturnedItem>> GetUnturnedItemsIdsAsync();
        Task<IEnumerable<UnturnedItem>> GetUnturnedItemsIdsNoIconAsync();
        Task<UnturnedItem> GetUnturnedItemAsync(int itemId);
    }
}
