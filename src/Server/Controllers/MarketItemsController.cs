using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.ApiKeyAuthentication;
using Marketplace.DatabaseProvider;
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Database;
using Marketplace.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Marketplace.DatabaseProvider.Extensions;


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
        public async Task<IActionResult> GetMarketItems()
        {
            return Ok(await marketPlaceRepository.GetMarketItemsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarketItem(int id)
        {
            return Ok(await marketPlaceRepository.GetMarketItemAsync(id));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> ChangePriceMarketItem(int id, [FromQuery] decimal price) //I think price should be in the body but I wanna hear what you think about that
        {
            MarketItem marketItem = await marketPlaceRepository.GetMarketItemAsync(id);
            if (marketItem.SellerId == User.Identity.Name && !marketItem.IsSold)
            {
                await marketPlaceRepository.ChangePriceMarketItemAsync(id, price);
                return Ok();
            }
            return Unauthorized();
        }

        [ApiKeyAuth]
        [HttpPost]
        public async Task<IActionResult> PostMarketItem([FromBody] MarketItem marketItem)
        {
            return Ok(await marketPlaceRepository.AddMarketItemAsync(marketItem));
        }

        [Authorize]
        [HttpPost("{id}/buy")]
        public async Task<bool> TryBuyMarketItem(int id)
        {
            decimal balance = await uconomyRepository.GetBalanceAsync(User.Identity.Name);
            MarketItem item = await marketPlaceRepository.GetMarketItemAsync(id);
            if (item != null && !item.IsSold && item.Price <= balance && item.SellerId != User.Identity.Name)
            {
                await uconomyRepository.IncreaseBalance(User.Identity.Name, item.Price * -1);
                await uconomyRepository.IncreaseBalance(item.SellerId, item.Price);
                await marketPlaceRepository.BuyMarketItemAsync(id, User.Identity.Name);
                return true;
            }
            return false;
        }

        [Authorize]
        [HttpGet("my")] //I'm not sure about this naming, I think it should be /items instead of /my
        public async Task<IActionResult> GetMyMarketItems()
        {
            return Ok(await marketPlaceRepository.GetPlayerMarketItemsAsync(User.Identity.Name));
        }

        [ApiKeyAuth]
        [HttpGet("{id}/claim")]
        public async Task<IActionResult> ClaimMarketItem(int id, [FromQuery] string playerId)
        {
            MarketItem marketItem = await marketPlaceRepository.GetMarketItemAsync(id);
            
            if (marketItem == null) //Ehm this feels wrong.
            {
                return Ok(new MarketItem());
            }

            if (playerId == marketItem.BuyerId && !marketItem.IsClaimed)
            {
                await marketPlaceRepository.ClaimMarketItemAsync(id);                
            }
            return Ok(marketItem);
        }

        [Authorize]
        [HttpGet("~/mybalance")]
        public async Task<IActionResult> GetMyBalance()
        {
            return Ok(await uconomyRepository.GetBalanceAsync(User.Identity.Name));
        }
    }
}