using Marketplace.DatabaseProvider.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Marketplace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly IServersRepository serversRepository;
        public ServersController(IServersRepository serversRepository)
        {
            this.serversRepository = serversRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetServersAsync()
        {
            return Ok(await serversRepository.GetServersAsync());
        }

        [HttpGet("{serverId}")]
        public async Task<IActionResult> GetServerAsync(int serverId)
        {
            return Ok(await serversRepository.GetServerAsync(serverId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostServerAsync([FromBody] Shared.Server server)
        {
            int serverId = await serversRepository.CreateServerAsync(server);
            if (serverId == 0)
                return BadRequest();
            else
                return Ok(serverId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> PutServerAsync([FromBody] Shared.Server server)
        {
            await serversRepository.UpdateServerAsync(server);
            return Ok();
        }
    }
}
    