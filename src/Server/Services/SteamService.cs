using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public class SteamService
    {
        private readonly SteamWebInterfaceFactory steamFactory;
        private readonly SteamUser steamUser;


        public SteamService(SteamWebInterfaceFactory steamFactory)
        {
            this.steamFactory = steamFactory;
            steamUser = steamFactory.CreateSteamWebInterface<SteamUser>(new HttpClient());
        }

        public async Task<string> GetPlayerNameAsync(string steamId)
        {
            var summaries = await steamUser.GetPlayerSummaryAsync(ulong.Parse(steamId));
            return summaries.Data.Nickname;
        }
    }
}
