using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.WebSockets;
using Marketplace.WebSockets;
using Marketplace.WebSockets.Attributes;
using Marketplace.WebSockets.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public class ServerService
    {
        private readonly IServersRepository serversRepository;
        private readonly WebSocketsManager webSocketsManager;
        private readonly ILogger<ServerService> logger;

        private List<Shared.Server> connectedServers = new List<Shared.Server>();

        public ServerService(IServersRepository serversRepository, ILogger<ServerService> logger)
        {
            this.serversRepository = serversRepository;
            this.webSocketsManager = new WebSocketsManager();
            this.logger = logger;
        }

        public void Initialize()
        {
            webSocketsManager.Initialize(GetType().Assembly, this);
        }

        [WebSocketCall("ServerId")]
        public async Task ConnectServerAsync(WebSocketMessage msg)
        {
            var server = await serversRepository.GetServerAsync((int)(long)msg.Arguments[0]);
            if (server == null)
            {
                logger.LogInformation($"Server with {msg.Arguments[0]} ID not found!");
                return;
            }

            // add to connected servers
            server.WebSocket = msg.WebSocket;
            connectedServers.Add(server);

            logger.LogInformation($"Server {server.ServerName} has connected to web");
        }

        public async Task ListenServerWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            try
            {
                await webSocketsManager.ListenWebSocketAsync(webSocket);
            } catch (Exception e)
            {
                logger.LogError(e, $"Connection with the server {context.Connection.RemoteIpAddress} lost");
            }
            connectedServers.RemoveAll(x => x.WebSocket == webSocket);
        }

        public async Task<decimal?> GetPlayerBalanceAsync(string steamId)
        {
            var server = connectedServers.FirstOrDefault();
            if (server == null)
            {
                logger.LogInformation("There isn't any server connected to website");
                return null;
            }

            var msg = await webSocketsManager.AskWebSocketAsync(server.WebSocket, "PlayerBalance", steamId);
            if (msg != null)
                return (decimal)(long)msg.Arguments[0];
            else
                return null;
        }
    }
}
