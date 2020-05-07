using Marketplace.Shared;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public interface IUserService
    {
        ValueTask<string> GetPlayerNameAsync(string steamId);
        ValueTask<UserInfo> GetUserInfoAsync(string steamId, string role, bool isAuthenticated);
    }
}
