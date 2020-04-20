using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public interface ISteamService
    {
        ValueTask<string> GetPlayerNameAsync(string steamId);
    }
}
