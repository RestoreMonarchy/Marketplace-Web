using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Services;
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
        private readonly ISettingService settingService;
        public SettingsController(ISettingService settingService)
        {
            this.settingService = settingService;
        }

        [HttpGet("{settingId}")] 
        public async Task<IActionResult> GetSettingAsync(string settingId)
        {
            return Ok(await settingService.GetSettingAsync(settingId));
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPut("{settingId}")]
        public async Task<IActionResult> UpdateSettingAsync(string settingId, [FromBody] Setting setting)
        {
            await settingService.UpdateSettingAsync(setting.SettingId, setting.SettingValue);
            return Ok();
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpGet]
        public async Task<IActionResult> GetSettingsAsync()
        {
            var settings = await settingService.GetSettingsAsync();
            if (Environment.GetEnvironmentVariable("ADMIN_STEAMID") == User.Identity.Name)
                return Ok(settings);
            else
                return Ok(settings.Where(x => !x.IsAdmin));            
        }
    }
}