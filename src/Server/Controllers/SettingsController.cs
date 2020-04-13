using System.Threading.Tasks;
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Shared;
using Marketplace.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepository settingsRepository;
        public SettingsController(ISettingsRepository settingsRepository)
        {
            this.settingsRepository = settingsRepository;
        }

        [HttpGet("{settingId}")] 
        public async Task<IActionResult> GetSettingAsync(string settingId)
        {
            return Ok(await settingsRepository.GetSettingAsync(settingId));
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPut("{settingId}")]
        public async Task<IActionResult> UpdateSettingAsync(string settingId, [FromBody] Setting setting)
        {
            await settingsRepository.UpdateSettingValueAsync(settingId, setting.SettingValue);
            return Ok();
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpGet]
        public async Task<IActionResult> GetSettingsAsync()
        {
            return Ok(await settingsRepository.GetSettingsAsync());
        }
    }
}