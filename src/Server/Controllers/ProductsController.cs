﻿using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Filters;
using Marketplace.Server.Services;
using Marketplace.Server.WebSockets.Data;
using Marketplace.Shared;
using Marketplace.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepository productsRepository;
        private readonly IEconomyWebSocketsData economyWebSocketsData;
        private readonly ISteamService steamService;

        public ProductsController(IProductsRepository productsRepository, IEconomyWebSocketsData economyWebSocketsData, ISteamService steamService)
        {
            this.productsRepository = productsRepository;
            this.economyWebSocketsData = economyWebSocketsData;
            this.steamService = steamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
            return Ok(await productsRepository.GetProductsAsync());
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPost]
        public async Task<IActionResult> PostProductAsync([FromBody] Product product)
        {
            int productId = await productsRepository.CreateProductAsync(product);
            if (productId == 0)
                return BadRequest();
            else
                return Ok(productId);
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPut]
        public async Task<IActionResult> PutProductAsync([FromBody] Product product)
        {
            await productsRepository.UpdateProductAsync(product);
            return Ok();
        }

        [HttpPost("{productId}/buy")]
        public async Task<IActionResult> PostBuyProductAsync(int productId, [FromQuery] int serverId)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var balance = await economyWebSocketsData.GetPlayerBalanceAsync(User.Identity.Name);
            if (!balance.HasValue)
                return BadRequest();

            var playerName = await steamService.GetPlayerNameAsync(User.Identity.Name);
            switch (await productsRepository.BuyProductAsync(productId, serverId, User.Identity.Name, playerName, balance.Value))
            {
                case 0:
                    var price = await productsRepository.GetProductPriceAsync(productId);
                    await economyWebSocketsData.IncrementBalanceAsync(User.Identity.Name, -price);
                    return Ok();
                case 1:
                    return NotFound();
                case 2:
                    return NoContent();
                case 3:
                    return StatusCode(StatusCodes.Status409Conflict);                    
                case 4:
                    return BadRequest();
            }
            return BadRequest();
        }

        [HttpGet("Transactions")]
        public async Task<IActionResult> GetProductTransactionsAsync([FromQuery] int top = 5)
        {
            return Ok(await productsRepository.GetProductTransactionsAsync(top));
        }

        [ApiKeyAuth]
        [HttpGet("Server")]
        public async Task<IActionResult> GetServerTransactionsAsync([FromQuery] int serverId)
        {
            if (serverId == 0)
                return BadRequest();

            return Ok(await productsRepository.GetServerTransactionsAsync(serverId));
        }
    }
}