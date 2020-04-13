
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Shared;
using Marketplace.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandsRepository commandsRepository;

        public CommandsController(ICommandsRepository commandsRepository)
        {
            this.commandsRepository = commandsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCommandsAsync()
        {
            return Ok(await commandsRepository.GetCommandsAsync());
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPost]
        public async Task<IActionResult> PostCommandAsync([FromBody] Command command)
        {
            int commandId = await commandsRepository.AddCommandAsync(command);
            if (commandId == 0)
                return BadRequest();
            else
                return Ok(commandId);
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPut]
        public async Task<IActionResult> PutCommandAsync([FromBody] Command command)
        {
            await commandsRepository.UpdateCommandAsync(command);
            return Ok();
        }
    }
}
