using System.Collections.Generic;
using ApiKeyAuthentication;
using DatabaseManager;
using Marketplace.Server.Database;
using Marketplace.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketItemsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabaseManager _databaseManager;
        private MySqlConnection serversConnection => new MySqlConnection(_configuration.GetConnectionString("ServersDatabase"));

        public MarketItemsController(IConfiguration configuration, IDatabaseManager databaseManager)
        {
            _configuration = configuration;
            _databaseManager = databaseManager;
        }

        [HttpGet]
        public List<MarketItem> GetMarketItems()
        {
            var marketitems = _databaseManager.GetMarketItems();
            return marketitems;
        }

        [HttpGet("{id}")]
        public MarketItem GetMarketItem(int id)
        {
            return _databaseManager.GetMarketItem(id);
        }

        [Authorize]
        [HttpPut("{id}")]
        public void ChangePriceMarketItem(int id, [FromQuery] decimal price)
        {
            MarketItem marketItem = _databaseManager.GetMarketItem(id);
            if (marketItem.SellerId == User.Identity.Name && !marketItem.IsSold)
            {
                _databaseManager.ChangePriceMarketItem(id, price);
            }            
        }

        [ApiKeyAuth]
        [HttpPost]
        public int PostMarketItem([FromBody] MarketItem marketItem)
        {
            return _databaseManager.AddMarketItem(marketItem);
        }

        [Authorize]
        [HttpPost("{id}/buy")]
        public bool TryBuyMarketItem(int id)
        {
            decimal balance = serversConnection.UconomyGetBalance(User.Identity.Name);
            MarketItem item = _databaseManager.GetMarketItem(id);
            if (item != null && !item.IsSold && item.Price <= balance && item.SellerId != User.Identity.Name)
            {
                serversConnection.UconomyPay(User.Identity.Name, item.Price * -1);
                serversConnection.UconomyPay(item.SellerId, item.Price);
                _databaseManager.BuyMarketItem(id, User.Identity.Name);
                return true;
            }
            return false;
        }

        [Authorize]
        [HttpGet("my")]
        public List<MarketItem> GetMyMarketItems()
        {
            return _databaseManager.GetPlayerMarketItems(User.Identity.Name);
        }

        [ApiKeyAuth]
        [HttpGet("{id}/claim")]
        public MarketItem ClaimMarketItem(int id, [FromQuery] string playerId)
        {
            MarketItem marketItem = _databaseManager.GetMarketItem(id);
            
            if (marketItem == null)
            {
                return new MarketItem();
            }

            if (playerId == marketItem.BuyerId && !marketItem.IsClaimed)
            {
                _databaseManager.ClaimMarketItem(id);                
            }
            return marketItem;
        }

        [Authorize]
        [HttpGet("~/mybalance")]
        public decimal GetMyBalance()
        {
            return serversConnection.UconomyGetBalance(User.Identity.Name);
        }
    }
}