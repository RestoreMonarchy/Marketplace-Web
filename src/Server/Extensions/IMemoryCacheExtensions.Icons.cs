using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Extensions
{
    public static class IMemoryCacheExtensions
    {
        private const string IconKey = "Icon_{0}";

        /// <summary>
        /// Gets the memory cached icon.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="id"></param>
        /// <param name="get"></param>
        /// <returns>Stream of the icon. Always dispose it.</returns>
        public static async Task<Stream> GetOrCreateIconAsync(this IMemoryCache cache, ushort id, Func<Task<Stream>> get)
        {
            string path = string.Format(IconKey, id);
            var icon = await cache.GetOrCreateAsync<byte[]>("", async (entry) =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                using (var stream = await get())
                {
                    byte[] result = new byte[stream.Length];
                    await stream.ReadAsync(result, (int)stream.Position, (int)stream.Length);
                    return result;
                }


            });
            return new MemoryStream(icon);
        }
    }
}
