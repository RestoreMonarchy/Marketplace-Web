using Marketplace.Server.Services;
using Marketplace.WebSockets;
using System;
using System.Threading.Tasks;

namespace Marketplace.Server.WebSockets.Data
{
    public class EconomyWebSocketsData : IEconomyWebSocketsData
    {
        private readonly IWebSocketsManager webSocketsManager;
        private readonly IServersService serversService;

        public EconomyWebSocketsData(IWebSocketsManager webSocketsManager, IServersService serversService)
        {
            this.webSocketsManager = webSocketsManager;
            this.serversService = serversService;
        }

        public async Task<decimal?> GetPlayerBalanceAsync(string steamId)
        {
            var server = serversService.GetConnectedServer();
            if (server == null)
                return null;

            var msg = await webSocketsManager.AskWebSocketAsync(server.WebSocket, "PlayerBalance", steamId);
            if (msg != null)
                return Convert.ToDecimal(msg.Arguments[0]);
            else
                return null;
        }

        public async Task<bool?> IncrementBalanceAsync(string steamId, decimal amount)
        {
            var server = serversService.GetConnectedServer();
            if (server == null)
                return null;

            var msg = await webSocketsManager.AskWebSocketAsync(server.WebSocket, "IncrementPlayerBalance", steamId, amount.ToString());
            if (msg != null && bool.TryParse(msg.Arguments[0], out bool result))
                return result;
            else
                return null;
        }

        public async Task<bool?> PayAsync(string senderId, string receiverId, decimal amount)
        {
            var server = serversService.GetConnectedServer();
            if (server == null)
                return null;

            var msg = await webSocketsManager.AskWebSocketAsync(server.WebSocket, "Pay", senderId, receiverId, amount.ToString());
            if (msg != null && bool.TryParse(msg.Arguments[0], out bool result))
                return result;
            else
                return null;
        }
    }
}
