using System;
using System.Collections.Generic;
using Marketplace.ApiKeyAuthentication;
using Marketplace.DatabaseProvider;
using Marketplace.Server.Models;
using Marketplace.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnturnedItemsController : ControllerBase
    {
        private readonly IDatabaseProvider _databaseProvider;
        private Dictionary<ushort, IconCache> cacheIcons = new Dictionary<ushort, IconCache>();

        public UnturnedItemsController(IDatabaseProvider databaseManager)
        {
            _databaseProvider = databaseManager;
        }

        [HttpGet]
        public List<UnturnedItem> GetUnturnedItems([FromQuery] bool onlyIds = false, [FromQuery] bool withNoIcons = false)
        {
            if (onlyIds)
            {
                if (withNoIcons)
                {
                    return _databaseProvider.GetUnturnedItemsIdsNoIcon();  
                } else
                {
                    return _databaseProvider.GetUnturnedItemsIds();
                }                
            }
            else
            {
                return _databaseProvider.GetUnturnedItems();
            }
        }

        [HttpGet("{itemId}")]
        public UnturnedItem GetUnturnedItem(ushort itemId)
        {
            return _databaseProvider.GetUnturnedItem(itemId);
        }

        [ApiKeyAuth]
        [HttpPost("{itemId}/icon")]
        public void AddIcon(ushort itemId, [FromBody] UnturnedItem item)
        {
            _databaseProvider.AddItemIcon(itemId, item.Icon);
        }

        [HttpGet("{itemId}/icon")]
        public IActionResult GetIcon(ushort itemId)
        {
            if (!cacheIcons.TryGetValue(itemId, out IconCache cache) || cache.LastUpdate.AddMinutes(10) > DateTime.Now)
            {
                cacheIcons[itemId] = new IconCache(_databaseProvider.GetItemIcon(itemId), DateTime.Now);
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
            _databaseProvider.AddUnturnedItem(item);
        }
    }
}