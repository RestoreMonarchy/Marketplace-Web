using System;
using System.Collections.Generic;
using ApiKeyAuthentication;
using DatabaseManager;
using Marketplace.Server.Models;
using Marketplace.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnturnedItemsController : ControllerBase
    {
        private readonly IDatabaseManager _databaseManager;
        private Dictionary<ushort, IconCache> cacheIcons = new Dictionary<ushort, IconCache>();

        public UnturnedItemsController(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [HttpGet]
        public List<UnturnedItem> GetUnturnedItems([FromQuery] bool onlyIds = false, [FromQuery] bool withNoIcons = false)
        {
            if (onlyIds)
            {
                if (withNoIcons)
                {
                    return _databaseManager.GetUnturnedItemsIdsNoIcon();  
                } else
                {
                    return _databaseManager.GetUnturnedItemsIds();
                }                
            }
            else
            {
                return _databaseManager.GetUnturnedItems();
            }
        }

        [HttpGet("{itemId}")]
        public UnturnedItem GetUnturnedItem(ushort itemId)
        {
            return _databaseManager.GetUnturnedItem(itemId);
        }

        [ApiKeyAuth]
        [HttpPost("{itemId}/icon")]
        public void AddIcon(ushort itemId, [FromBody] UnturnedItem item)
        {
            _databaseManager.AddItemIcon(itemId, item.Icon);
        }

        [HttpGet("{itemId}/icon")]
        public IActionResult GetIcon(ushort itemId)
        {
            if (!cacheIcons.TryGetValue(itemId, out IconCache cache) || cache.LastUpdate.AddMinutes(10) > DateTime.Now)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Getting icon for {itemId} ");
                Console.ResetColor();
                cacheIcons[itemId] = new IconCache(_databaseManager.GetItemIcon(itemId), DateTime.Now);
            }

            if (cacheIcons[itemId].Data != null)
            {                
                return File(cacheIcons[itemId].Data, "image/png");
            }
            else
            {
                return File(new byte[0] { }, "image/png");
            }
        }

        [ApiKeyAuth]
        [HttpPost]        
        public void AddUnturnedItems([FromBody] UnturnedItem item)
        {
            _databaseManager.AddUnturnedItem(item);
        }
    }
}