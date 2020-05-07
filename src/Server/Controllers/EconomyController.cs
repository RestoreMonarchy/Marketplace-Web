using Marketplace.Server.WebSockets.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public EconomyController(IEconomyWebSocketsData economyWebSocketsData)
        {
            this.economyWebSocketsData = economyWebSocketsData;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMyBalanceAsync()
        {
            try
            {
                var playerBalance = await economyWebSocketsData.GetPlayerBalanceAsync(User.Identity.Name);
                if (playerBalance == null)
                    return StatusCode(StatusCodes.Status503ServiceUnavailable);
                else
                    return Ok(playerBalance);
            } catch (TimeoutException)
            {
                return StatusCode((int)HttpStatusCode.GatewayTimeout);
            }
        }
    }
}
