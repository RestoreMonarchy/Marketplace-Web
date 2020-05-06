using Marketplace.Server.WebSockets.Data;
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
                var playerBalance = await economyWebSocketsData.GetPlayerBalanceAsync(User.Identity.Name);
                if (playerBalance == null)
                    return NoContent();
                else
                    return Ok(playerBalance);
            } catch (TimeoutException)
            {
                return StatusCode((int)HttpStatusCode.GatewayTimeout);
            }
        }
    }
}
