using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Marketplace.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UconomyController : ControllerBase
    {
        private readonly IUconomyRepository uconomyRepository;
        public UconomyController(IUconomyRepository uconomyRepository)
        {
            this.uconomyRepository = uconomyRepository;
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
                Console.WriteLine(e);
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
                Console.WriteLine(e);
                return Ok(0);
            }            
        }
    }
}
