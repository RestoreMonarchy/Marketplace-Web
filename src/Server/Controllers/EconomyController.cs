using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Services;
using Marketplace.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EconomyController : ControllerBase
    {
        private readonly IEconomyRepository economyRepository;
        private ServerService serverService;
        private readonly ILogger<EconomyController> logger;
        public EconomyController(IEconomyRepository economyRepository, ServerService serverService, ILogger<EconomyController> logger)
        {
            this.economyRepository = economyRepository;
            this.serverService = serverService;
            this.logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMyBalanceAsync()
        {
            try
            {
                return Ok(await serverService.GetPlayerBalanceAsync(User.Identity.Name));
            } catch (Exception e)
            {
                // TODO: Use logger instead here too
                LogUconomyConnectionError(e);
                return Ok(0);
            }            
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpGet("total")]
        public async Task<IActionResult> GetTotalBalanceAsync()
        {
            try
            {
                return Ok(await economyRepository.GetTotalBalanceAsync());
            } catch (Exception e)
            {
                LogUconomyConnectionError(e);
                return Ok(0);
            }            
        }

        private void LogUconomyConnectionError(Exception e)
        {
            logger.LogError("Error occured while communicating with uconomy database: {ex}", e);
        }
    }
}
