using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Marketplace.ApiKeyAuthentication;
using Marketplace.DatabaseProvider;
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Extensions;
using Marketplace.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnturnedItemsController : ControllerBase
    {
        private readonly IUnturnedItemAssetsRepository unturnedItemAssetsRepository;
        private readonly IMemoryCache memoryCache;

        public UnturnedItemsController(IUnturnedItemAssetsRepository unturnedItemAssetsRepository,
            IMemoryCache memoryCache)
        {
            this.unturnedItemAssetsRepository = unturnedItemAssetsRepository;
            this.memoryCache = memoryCache;
        }


        [HttpGet]
        public async Task<IActionResult> GetUnturnedItemsAsync([FromQuery] bool onlyIds = false, [FromQuery] bool withNoIcons = false)
        {
            if (onlyIds)
            {
                if (withNoIcons)
                {
                    return Ok(await unturnedItemAssetsRepository.GetUnturnedItemsIdsNoIconAsync());  
                } else
                {
                    return Ok(await unturnedItemAssetsRepository.GetUnturnedItemsIdsNoIconAsync());
                }                
            }
            else
            {
                return Ok(await unturnedItemAssetsRepository.GetUnturnedItemsAsync());
            }
        }

        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetUnturnedItemAsync([FromRoute] ushort itemId)
        {
            return Ok(await unturnedItemAssetsRepository.GetUnturnedItemAsync(itemId));
        }

        [ApiKeyAuth]
        [HttpPost("{itemId}/icon")]
        public async Task AddIconAsync([FromRoute] ushort itemId, [FromBody] UnturnedItem item)
        {
            using var stream = new MemoryStream(item.Icon);
            await unturnedItemAssetsRepository.AddItemIconAsync(itemId, stream);
        }

        [HttpGet("{itemId}/icon")]
        public async Task<IActionResult> GetIcon([FromRoute] ushort itemId)
        {
            using var icon = await memoryCache.GetOrCreateIconAsync(itemId, async () =>
            {
                return await unturnedItemAssetsRepository.GetItemIconAsync(itemId);
            });

            return Ok(File(icon, "image/png"));
        }

        [ApiKeyAuth]
        [HttpPost]        
        public async Task AddUnturnedItemsAsync([FromBody] UnturnedItem item)
        {
            await unturnedItemAssetsRepository.AddUnturnedItemAsync(item);
        }
    }
}