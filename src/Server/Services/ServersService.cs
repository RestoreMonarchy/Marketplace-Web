using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public class ServerService
    {
        private readonly IServersRepository serversRepository;
        private readonly WebSocketsUtility webSocketsService;
        private readonly ILogger<ServerService> logger;

        private List<Shared.Server> connectedServers = new List<Shared.Server>();

        public ServerService(IServersRepository serversRepository, WebSocketsUtility webSocketsService, ILogger<ServerService> logger)
        {
            this.serversRepository = serversRepository;
            this.webSocketsService = webSocketsService;
            this.logger = logger;
        }

        public async Task ListenServerWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            var server = await GetServerAsync(webSocket);
            if (server == null)
            {
                logger.LogInformation("Server didnt respond or returned ID could not be found");
                return;
            }

            // add to connected servers
            server.WebSocket = webSocket;
            connectedServers.Add(server);

            logger.LogInformation($"Server {server.ServerName} has " +
                $"connected to web socket from IP {context.Connection.RemoteIpAddress}");
            // start listening to server
            await webSocketsService.ListenWebSocketAsync(context, webSocket);

            // when stops listening (means client quit or crashed)
            server.WebSocket = null;
            connectedServers.Remove(server);
        }

        private async Task<Shared.Server> GetServerAsync(WebSocket webSocket)
        {
            var msg = await webSocketsService.AskWebSocketAsync(webSocket, "ServerId");

            if (msg != null && int.TryParse(msg, out int serverId))
            {
                return await serversRepository.GetServerAsync(serverId);
            }
            return null;
        }

        public async Task<decimal?> GetPlayerBalanceAsync(string steamId)
        {
            var server = connectedServers.FirstOrDefault();
            if (server == null)
            {
                logger.LogInformation("There isn't any server connected to website");
                return null;
            }

            var msg = await webSocketsService.AskWebSocketAsync(server.WebSocket, "PlayerBalance", steamId);
            if (msg != null && decimal.TryParse(msg, out var balance))
                return balance;
            else
                return null;
        }
    }
}
