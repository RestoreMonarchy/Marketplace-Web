using System.Threading.Tasks;
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Marketplace.Server.Filters;
using Marketplace.Server.WebSockets.Data;
using Marketplace.Server.Services;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketItemsController : ControllerBase
    {
        private readonly IMarketItemsRepository marketItemsRepository;
        private readonly IEconomyWebSocketsData economyWebSocketsData;
        private readonly IUserService userService;

        public MarketItemsController(IMarketItemsRepository marketItemsRepository , IEconomyWebSocketsData economyWebSocketsData, 
            IUserService userService)
        {
            this.marketItemsRepository = marketItemsRepository;
            this.economyWebSocketsData = economyWebSocketsData;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMarketItemsAsync()
        {
            return Ok(await marketItemsRepository.GetMarketItemsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarketItemAsync(int id)
        {
            return Ok(await marketItemsRepository.GetMarketItemAsync(id));
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

            if (!decimal.TryParse(content, out decimal price) || price < 0)
            {
                return BadRequest();
            }

            switch (await marketItemsRepository.ChangePriceMarketItemAsync(id, User.Identity.Name, price))
            {
                case 0:
                    return Ok();
                case 1:
                    return NotFound();
                case 2:
                    return StatusCode(StatusCodes.Status410Gone);
                case 3:
                    return StatusCode(StatusCodes.Status403Forbidden);
            }

            return BadRequest();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> TakeDownMarketItemAsync(int id)
        {
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine("ID IS: " + id);
            System.Console.WriteLine();
            System.Console.WriteLine();
            var playerName = await userService.GetPlayerNameAsync(User.Identity.Name);
            var result = await marketItemsRepository.TakeDownMarketItemAsync(id, User.Identity.Name, playerName);
            switch (result)
            {
                case 0:
                    return Ok();
                case 1:
                    return StatusCode(StatusCodes.Status405MethodNotAllowed);
                case 2:
                    return NotFound();
                case 3:
                    return StatusCode(StatusCodes.Status410Gone);
                case 4:
                    return StatusCode(StatusCodes.Status403Forbidden);
            }
            return BadRequest();
        }

        [ApiKeyAuth]
        [HttpPost]
        public async Task<IActionResult> SellMarketItemAsync([FromBody] MarketItem marketItem)
        {
            switch (await marketItemsRepository.SellMarketItemAsync(marketItem))
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
            switch (await marketItemsRepository.BuyMarketItemAsync(id, User.Identity.Name))
            {
                case 0:
                    var item = await marketItemsRepository.GetMarketItemAsync(id);
                    var result = await economyWebSocketsData.PayAsync(User.Identity.Name, item.SellerId, item.Price);
                    if (!result.HasValue)
                        return StatusCode(StatusCodes.Status503ServiceUnavailable);
                    
                    if (result.Value)
                    {
                        var playerName = await userService.GetPlayerNameAsync(User.Identity.Name);
                        await marketItemsRepository.FinishBuyMarketItemAsync(id, User.Identity.Name, playerName);
                        return Ok();
                    }
                    else
                        return BadRequest();
                case 1:
                    return NotFound();
                case 2:
                    return StatusCode(StatusCodes.Status403Forbidden);
                case 3:
                    return StatusCode(StatusCodes.Status410Gone);
                case 4:
                    return StatusCode(StatusCodes.Status409Conflict);
            }

            return BadRequest();
        }

        [Authorize]
        [HttpGet("buyer")]
        public async Task<IActionResult> GetBuyerMarketItemsAsync()
        {
            return Ok(await marketItemsRepository.GetBuyerMarketItemsAsync(User.Identity.Name));
        }

        [Authorize]
        [HttpGet("seller")]
        public async Task<IActionResult> GetSellerMarketItemsAsync()
        {
            return Ok(await marketItemsRepository.GetSellerMarketItemsAsync(User.Identity.Name));
        }

        [ApiKeyAuth]
        [HttpGet("{id}/claim")]
        public async Task<IActionResult> ClaimMarketItemAsync(int id, [FromQuery] string playerId)
        {
            MarketItem marketItem = await marketItemsRepository.GetMarketItemAsync(id);

            if (marketItem == null)
            {
                return NotFound();
            }

            if (playerId != marketItem.BuyerId)
                return Unauthorized();

            if (marketItem.IsClaimed)
                return BadRequest();

            await marketItemsRepository.ClaimMarketItemAsync(id);
            return Ok(marketItem);
        }
    }
}