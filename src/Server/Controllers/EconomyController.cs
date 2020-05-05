using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Services;
using Marketplace.Server.WebSockets.Data;
using Marketplace.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EconomyController : ControllerBase
    {
        private IEconomyWebSocketsData economyWebSocketsData;
        private readonly ILogger<EconomyController> logger;
        public EconomyController(IEconomyWebSocketsData economyWebSocketsData, ILogger<EconomyController> logger)
        {
            this.economyWebSocketsData = economyWebSocketsData;
            this.logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMyBalanceAsync()
        {
            try
            {
                return Ok(await economyWebSocketsData.GetPlayerBalanceAsync(User.Identity.Name));
            } catch (Exception e)
            {
                if (e as TimeoutException != null)
                {
                    return StatusCode((int)HttpStatusCode.GatewayTimeout);
                } else
                {
                    LogEconomyConnectionError(e);
                    return Ok(0);
                }
            }            
        }
        
        private void LogEconomyConnectionError(Exception e)
        {
            logger.LogError("Error occured while communicating with uconomy database: {ex}", e);
        }
    }
}
