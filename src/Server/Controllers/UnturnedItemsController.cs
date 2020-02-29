using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Marketplace.ApiKeyAuthentication;
using Marketplace.DatabaseProvider;
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Extensions;
using Marketplace.Shared;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type =typeof(IEnumerable<UnturnedItem>))]
        public async Task<IActionResult> GetUnturnedItemsAsync([FromQuery] bool onlyIds = false)
        {
            if (onlyIds)
                return Ok(await unturnedItemAssetsRepository.GetUnturnedItemsIdsNoIconAsync());
            return Ok(await unturnedItemAssetsRepository.GetUnturnedItemsAsync());
        }

        [HttpGet("{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UnturnedItem))]
        public async Task<IActionResult> GetUnturnedItemAsync([FromRoute] ushort itemId)
        {
            return Ok(await unturnedItemAssetsRepository.GetUnturnedItemAsync(itemId));
        }

        [ApiKeyAuth]
        [HttpPut("{itemId}/icon")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SetIconAsync([FromRoute] ushort itemId, [FromBody] UnturnedItem item)
        {
            using var stream = new MemoryStream(item.Icon);
            await unturnedItemAssetsRepository.SetIconAsync(itemId, stream); //Mabye
            return Ok();
        }

        [HttpGet("{itemId}/icon")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        public async Task<IActionResult> GetIcon([FromRoute] ushort itemId)
        {
            using var icon = await memoryCache.GetOrCreateIconAsync(itemId, async () =>
            {
                return await unturnedItemAssetsRepository.GetItemIconAsync(itemId);
            });
            if (icon.Length == 0)
                return NotFound();
            return Ok(File(icon, "image/png"));
        }

        [ApiKeyAuth]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UnturnedItem))]
        public async Task<IActionResult> AddUnturnedItemsAsync([FromBody] UnturnedItem item)
        {
            if (await unturnedItemAssetsRepository.GetUnturnedItemAsync(item.ItemId) != null)
                return Conflict();

            await unturnedItemAssetsRepository.AddUnturnedItemAsync(item);
            return Ok(item);
        }
    }
}