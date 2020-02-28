using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Marketplace.ApiKeyAuthentication;
using Marketplace.DatabaseProvider;
using Marketplace.Server.Extensions;
using Marketplace.Server.Models;
using Marketplace.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnturnedItemsController : ControllerBase
    {
        private readonly IDatabaseProvider databaseProvider;
        private readonly IMemoryCache memoryCache;

        public UnturnedItemsController(IDatabaseProvider databaseProvider, IMemoryCache memoryCache)
        {
            this.databaseProvider = databaseProvider;
            this.memoryCache = memoryCache;
        }


        [HttpGet]
        public async Task<IActionResult> GetUnturnedItems([FromQuery] bool onlyIds = false, [FromQuery] bool withNoIcons = false)
        {
            if (onlyIds)
            {
                if (withNoIcons)
                {
                    return Ok(await databaseProvider.GetUnturnedItemsIdsNoIconAsync());  
                } else
                {
                    return Ok(await databaseProvider.GetUnturnedItemsIdsNoIconAsync());
                }                
            }
            else
            {
                return Ok(await databaseProvider.GetUnturnedItemsAsync());
            }
        }

        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetUnturnedItem([FromRoute] ushort itemId)
        {
            return Ok(await databaseProvider.GetUnturnedItemAsync(itemId));
        }

        [ApiKeyAuth]
        [HttpPost("{itemId}/icon")]
        public async Task AddIcon([FromRoute] ushort itemId, [FromBody] UnturnedItem item)
        {
            await databaseProvider.AddItemIconAsync(itemId, item.Icon);
        }

        [HttpGet("{itemId}/icon")]
        public async Task<IActionResult> GetIcon([FromRoute] ushort itemId)
        {
            using var icon = await memoryCache.GetOrCreateIconAsync(itemId, async () =>
            {
                return new MemoryStream(await databaseProvider.GetItemIconAsync(itemId));
            });

            return Ok(File(icon, "image/png"));
        }

        [ApiKeyAuth]
        [HttpPost]        
        public async Task AddUnturnedItems([FromBody] UnturnedItem item)
        {
            await databaseProvider.AddUnturnedItemAsync(item);
        }
    }
}