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
            server.Id = await serversRepository.AddServerAsync(server);
            return Ok(server);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{serverId}")]
        public async Task<IActionResult> DeleteServerAsync(int serverId)
        {
            await serversRepository.DeleteServerAsync(serverId);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{serverId}")]
        public async Task<IActionResult> PutServerAsync(int serverId)
        {
            await serversRepository.ToggleServerAsync(serverId);
            return Ok();
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPatch]
        public async Task<IActionResult> PatchServerAsync([FromBody] Shared.Server server)
        {
            await serversRepository.UpdateServerAsync(server);
            return Ok();
        }
    }
}
    