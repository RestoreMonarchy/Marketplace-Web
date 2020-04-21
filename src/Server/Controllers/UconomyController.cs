using Marketplace.DatabaseProvider.Repositories;
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
    public class UconomyController : ControllerBase
    {
        private readonly IEconomyRepository uconomyRepository;
        private readonly ILogger<UconomyController> logger;
        public UconomyController(IEconomyRepository uconomyRepository, ILogger<UconomyController> logger)
        {
            this.uconomyRepository = uconomyRepository;
            this.logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMyBalanceAsync()
        {
            try
            {
                return Ok(await uconomyRepository.GetBalanceAsync(User.Identity.Name));
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
                return Ok(await uconomyRepository.GetTotalBalanceAsync());
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
