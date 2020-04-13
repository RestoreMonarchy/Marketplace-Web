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
using System.Net;
using System.IO;
using Marketplace.Shared.Constants;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketItemsController : ControllerBase
    {
        private readonly IMarketItemsRepository marketPlaceRepository;
        private readonly IUconomyRepository uconomyRepository;

        public MarketItemsController(IMarketItemsRepository marketPlaceRepository, IUconomyRepository uconomyRepository)
        {
            this.marketPlaceRepository = marketPlaceRepository;
            this.uconomyRepository = uconomyRepository;
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

            System.Console.WriteLine();
            System.Console.WriteLine("contents: " + content);
            System.Console.WriteLine();

            if (!decimal.TryParse(content, out decimal price))
            {
                return BadRequest();
            }
            System.Console.WriteLine();
            System.Console.WriteLine($"price is {price}");
            System.Console.WriteLine();

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
            decimal balance = await uconomyRepository.GetBalanceAsync(User.Identity.Name);

            switch (await marketPlaceRepository.BuyMarketItemAsync(id, User.Identity.Name, balance))
            {
                case 0:
                    MarketItem item = await marketPlaceRepository.GetMarketItemAsync(id);
                    await uconomyRepository.IncreaseBalance(User.Identity.Name, item.Price * -1);
                    await uconomyRepository.IncreaseBalance(item.SellerId, item.Price);
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

        [Authorize]
        [HttpGet("balance")]
        public async Task<IActionResult> GetMyBalanceAsync()
        {
            return Ok(await uconomyRepository.GetBalanceAsync(User.Identity.Name));
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpGet("balance/total")]
        public async Task<IActionResult> GetTotalBalanceAsync()
        {
            return Ok(await uconomyRepository.GetTotalBalanceAsync());
        }
    }
}