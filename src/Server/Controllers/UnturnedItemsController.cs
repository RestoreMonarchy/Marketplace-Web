using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Marketplace.ApiKeyAuthentication;
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnturnedItemsController : ControllerBase
    {
        private readonly IUnturnedItemAssetsRepository unturnedItemAssetsRepository;

        public UnturnedItemsController(IUnturnedItemAssetsRepository unturnedItemAssetsRepository)
        {
            this.unturnedItemAssetsRepository = unturnedItemAssetsRepository;
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
        public async Task<IActionResult> GetIconAsync([FromRoute] ushort itemId)
        {
            var icon = await unturnedItemAssetsRepository.GetItemIconAsync(itemId);

            if (icon.Length == 0)
                return NotFound();

            if (!icon.CanTimeout)
            {
                byte[] buffer = new byte[icon.Length];
                await icon.ReadAsync(buffer, 0, buffer.Length);
                return File(buffer, "image/png");
            }
            return File(icon, "image/png");
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