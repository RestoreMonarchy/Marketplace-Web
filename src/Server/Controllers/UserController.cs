using System.Security.Claims;
using System.Threading.Tasks;
using Marketplace.Server.Services;
using Marketplace.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userInfo = await userService.GetUserInfoAsync(User.Identity.Name, User.FindFirst(ClaimTypes.Role).Value, true);
                return Ok(userInfo);
            }
            else
            {
                return Ok(new UserInfo() { IsAuthenticated = false });
            }
        }

        [HttpGet("{playerId}")]
        public async Task<IActionResult> GetPlayerUserAsync(string playerId)
        {
            var userInfo = await userService.GetUserInfoAsync(playerId, "", false);
            return Ok(userInfo);
        }

        [HttpPost("~/signin")]
        public IActionResult SignIn(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "Steam");
        }

        [HttpGet("~/signout"), HttpPost("~/signout")]
        public IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}