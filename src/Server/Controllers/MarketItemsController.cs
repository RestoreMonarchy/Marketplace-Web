using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.ApiKeyAuthentication;
using Marketplace.DatabaseProvider;
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Marketplace.DatabaseProvider.Extensions;
using Microsoft.AspNetCore.Http;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketItemsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IMarketPlaceRepository marketPlaceRepository;
        private readonly IUconomyRepository uconomyRepository;

        public MarketItemsController(IConfiguration configuration, IMarketPlaceRepository marketPlaceRepository, IUconomyRepository uconomyRepository)
        {
            this.configuration = configuration;
            this.marketPlaceRepository = marketPlaceRepository;
            this.uconomyRepository = uconomyRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MarketItem>))]
        public async Task<IActionResult> GetMarketItemsAsync()
        {
            return Ok(await marketPlaceRepository.GetMarketItemsAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MarketItem))]
        public async Task<IActionResult> GetMarketItemAsync(int id)
        {
            return Ok(await marketPlaceRepository.GetMarketItemAsync(id));
        }

        [Authorize]
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MarketItem))]
        public async Task<IActionResult> ChangePriceMarketItemAsync(int id, [FromQuery] decimal price) //I think price should be in the body but I wanna hear what you think about that
        {
            MarketItem marketItem = await marketPlaceRepository.GetMarketItemAsync(id);
            if (marketItem.IsSold)
                return BadRequest();

            if (marketItem.SellerId != User.Identity.Name)
                return Unauthorized();


            await marketPlaceRepository.ChangePriceMarketItemAsync(id, price);
            marketItem.Price = price;

            return Ok(marketItem);

        }

        [ApiKeyAuth]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MarketItem))]
        public async Task<IActionResult> PostMarketItemAsync([FromBody] MarketItem marketItem)
        {
            return Ok(await marketPlaceRepository.AddMarketItemAsync(marketItem));
        }

        [Authorize]
        [HttpPost("{id}/buy")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemInfo))]
        public async Task<IActionResult> BuyMarketItemAsync(int id)
        {
            decimal balance = await uconomyRepository.GetBalanceAsync(User.Identity.Name);
            MarketItem item = await marketPlaceRepository.GetMarketItemAsync(id);
            if (item == null)
                return NotFound();
            if (item.IsSold)
                return BadRequest();
            if (balance < item.Price)
                return BadRequest();

            if (item.SellerId == User.Identity.Name)
                return Unauthorized();


            await uconomyRepository.IncreaseBalance(User.Identity.Name, item.Price * -1);
            await uconomyRepository.IncreaseBalance(item.SellerId, item.Price);
            await marketPlaceRepository.BuyMarketItemAsync(id, User.Identity.Name);
            return Ok(item.ItemInfo);

        }

        [Authorize]
        [HttpGet("items")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MarketItem>))]
        public async Task<IActionResult> GetMyMarketItemsAsync()
        {
            return Ok(await marketPlaceRepository.GetPlayerMarketItemsAsync(User.Identity.Name));
        }

        [ApiKeyAuth]
        [HttpPost("{id}/claim")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MarketItem))]
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
            return CreatedAtAction(nameof(GetMarketItemAsync), new { id = marketItem.Id }, marketItem);

        }

        [Authorize]
        [HttpGet("balance")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(decimal))]
        public async Task<IActionResult> GetMyBalanceAsync()
        {
            return Ok(await uconomyRepository.GetBalanceAsync(User.Identity.Name));
        }
    }
}