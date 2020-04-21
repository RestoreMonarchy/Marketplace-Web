using System.Threading.Tasks;
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Marketplace.DatabaseProvider.Extensions;
using Microsoft.AspNetCore.Http;
using System.IO;
using Marketplace.Shared.Constants;
using Marketplace.Server.Filters;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketItemsController : ControllerBase
    {
        private readonly IMarketItemsRepository marketPlaceRepository;
        private readonly IEconomyRepository economyRepository;

        public MarketItemsController(IMarketItemsRepository marketPlaceRepository, IEconomyRepository economyRepository)
        {
            this.marketPlaceRepository = marketPlaceRepository;
            this.economyRepository = economyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMarketItemsAsync()
        {
            return Ok(await marketPlaceRepository.GetMarketItemsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarketItemAsync(int id)
        {
            return Ok(await marketPlaceRepository.GetMarketItemAsync(id));
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> ChangePriceMarketItemAsync(int id) 
        {
            string content;
            using (var reader = new StreamReader(Request.Body))
            {
                content = await reader.ReadToEndAsync();
            }

            if (!decimal.TryParse(content, out decimal price))
            {
                return BadRequest();
            }

            switch (await marketPlaceRepository.ChangePriceMarketItemAsync(id, User.Identity.Name, price))
            {
                case 0:
                    return Ok();
                case 1:
                    return BadRequest();
                case 2:
                    return Unauthorized();
            }

            return BadRequest();
        }

        [ApiKeyAuth]
        [HttpPost]
        public async Task<IActionResult> SellMarketItemAsync([FromBody] MarketItem marketItem)
        {
            switch (await marketPlaceRepository.SellMarketItemAsync(marketItem))
            {
                case 0:
                    return Ok();
                case 1:
                    return StatusCode(StatusCodes.Status409Conflict);
            }

            return BadRequest();
        }

        [Authorize]
        [HttpPost("{id}/buy")]
        public async Task<IActionResult> BuyMarketItemAsync(int id)
        {
            decimal balance = await economyRepository.GetBalanceAsync(User.Identity.Name);

            switch (await marketPlaceRepository.BuyMarketItemAsync(id, User.Identity.Name, balance))
            {
                case 0:
                    MarketItem item = await marketPlaceRepository.GetMarketItemAsync(id);
                    await economyRepository.PayAsync(User.Identity.Name, item.SellerId, item.Price);
                    return Ok();
                case 1:                    
                    return NotFound();
                case 2:
                    return Unauthorized();
                case 3:
                    return NoContent();
                case 4:
                    return BadRequest();
                case 5:
                    return StatusCode(StatusCodes.Status409Conflict);
            }

            return BadRequest();
        }

        [Authorize]
        [HttpGet("trunk")]
        public async Task<IActionResult> GetMyMarketItemsAsync()
        {
            return Ok(await marketPlaceRepository.GetPlayerMarketItemsAsync(User.Identity.Name));
        }

        [ApiKeyAuth]
        [HttpGet("{id}/claim")]
        public async Task<IActionResult> ClaimMarketItemAsync(int id, [FromQuery] string playerId)
        {
            MarketItem marketItem = await marketPlaceRepository.GetMarketItemAsync(id);

            if (marketItem == null)
            {
                return NotFound();
            }

            if (playerId != marketItem.BuyerId)
                return Unauthorized();

            if (marketItem.IsClaimed)
                return BadRequest();

            await marketPlaceRepository.ClaimMarketItemAsync(id);
            return Ok(marketItem);
        }
    }
}