using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Filters;
using Marketplace.Server.Services;
using Marketplace.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnturnedItemsController : ControllerBase
    {
        private readonly IUnturnedItemsRepository unturnedItemsRepository;
        private readonly IUnturnedItemsIconService unturnedItemsIconService;

        public UnturnedItemsController(IUnturnedItemsRepository unturnedItemsRepository, IUnturnedItemsIconService unturnedItemsIconService)
        {
            this.unturnedItemsRepository = unturnedItemsRepository;
            this.unturnedItemsIconService = unturnedItemsIconService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UnturnedItem>))]
        public async Task<IActionResult> GetUnturnedItemsAsync([FromQuery] bool haveNoIcons = false)
        {
            if (haveNoIcons)
                return Ok(await unturnedItemsRepository.GetUnturnedItemsIdsNoIconAsync());
            return Ok(await unturnedItemsRepository.GetUnturnedItemsAsync());
        }

        [HttpGet("{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UnturnedItem))]
        public async Task<IActionResult> GetUnturnedItemAsync([FromRoute] ushort itemId)
        {
            return Ok(await unturnedItemsRepository.GetUnturnedItemAsync(itemId));
        }

        [ApiKeyAuth]
        [HttpPut("{itemId}/icon")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SetIconAsync([FromRoute] int itemId, [FromBody] UnturnedItem item)
        {
            await unturnedItemsIconService.UpdateIconAsync(itemId, item.Icon);
            return Ok();
        }

        [HttpGet("{itemId}/icon")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        public async Task<IActionResult> GetIconAsync([FromRoute] int itemId)
        {
            var icon = await unturnedItemsIconService.GetIconAsync(itemId);
            if (icon == null)
                return NotFound();
            return File(icon, "image/png");
        }

        [ApiKeyAuth]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UnturnedItem))]
        public async Task<IActionResult> AddUnturnedItemsAsync([FromBody] UnturnedItem item)
        {
            if (await unturnedItemsRepository.GetUnturnedItemAsync(item.ItemId) != null)
                return Conflict();

            await unturnedItemsRepository.AddUnturnedItemAsync(item);
            return Ok(item);
        }
    }
}