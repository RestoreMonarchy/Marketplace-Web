using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Constants;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public class UnturnedItemsIconService : IUnturnedItemsIconService
    {
        private readonly IUnturnedItemsRepository unturnedItemsRepository;
        private readonly IMemoryCache memoryCache;

        public UnturnedItemsIconService(IUnturnedItemsRepository unturnedItemsRepository, IMemoryCache memoryCache)
        {
            this.unturnedItemsRepository = unturnedItemsRepository;
            this.memoryCache = memoryCache;
        }

        public async Task<byte[]> GetIconAsync(int itemId)
        {
            return await memoryCache.GetOrCreateAsync(CacheKeys.ItemIconId(itemId), async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                return await unturnedItemsRepository.GetItemIconAsync(itemId);
            });
        }

        public async Task UpdateIconAsync(int itemId, byte[] icon)
        {
            await unturnedItemsRepository.SetIconAsync(itemId, icon);
            memoryCache.Remove(CacheKeys.ItemIconId(itemId));
        }
    }
}
