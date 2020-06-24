using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Services;
using Marketplace.Shared.Constants;
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
        private readonly IServersService serversService;
        public ServersController(IServersRepository serversRepository, IServersService serversService)
        {
            this.serversRepository = serversRepository;
            this.serversService = serversService;
        }

        [HttpGet]
        public async Task<IActionResult> GetServersAsync()
        {
            var servers = await serversRepository.GetServersAsync();
            serversService.ToggleConnectedServers(servers);
            return Ok(servers);
        }

        [HttpGet("{serverId}")]
        public async Task<IActionResult> GetServerAsync(int serverId)
        {
            var server = await serversRepository.GetServerAsync(serverId);
            if (serversService.GetConnectedServer(serverId) != null)
                server.IsConnected = true;
            else
                server.IsConnected = false;
            return Ok(server);
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPost]
        public async Task<IActionResult> PostServerAsync([FromBody] Shared.Server server)
        {
            server = await serversRepository.CreateServerAsync(server);
            if (server == null)
                return BadRequest();
            else
                return Ok(server);
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPut]
        public async Task<IActionResult> PutServerAsync([FromBody] Shared.Server server)
        {
            await serversRepository.UpdateServerAsync(server);
            return Ok();
        }
    }
}
    