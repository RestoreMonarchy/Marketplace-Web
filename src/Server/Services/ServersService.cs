using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.WebSockets;
using Marketplace.Shared;
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
    public class ServerService : IServersService
    {
        private readonly IServersRepository serversRepository;
        private readonly ILogger<ServerService> logger;
        private readonly IWebSocketsManager webSocketsManager;

        public IEnumerable<Shared.Server> ConnectedServers => connectedServers;

        public List<Shared.Server> connectedServers = new List<Shared.Server>();

        public ServerService(IServersRepository serversRepository, IWebSocketsManager webSocketsManager, ILogger<ServerService> logger)
        {
            this.serversRepository = serversRepository;
            this.webSocketsManager = webSocketsManager;
            this.logger = logger;
        }

        [WebSocketCall("ServerId")]
        private async Task ConnectServerAsync(WebSocketMessage msg)
        {
            var serverId = Convert.ToInt32(msg.Arguments[0]);
            var server = await serversRepository.GetServerAsync(serverId);
            if (server == null)
            {
                logger.LogInformation($"Server with {serverId} ID not found!");
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

        public Shared.Server GetConnectedServer(int? id = null)
        {
            Shared.Server server;
            if (!id.HasValue)
            {
                server = ConnectedServers.FirstOrDefault();
                if (server == null)
                    logger.LogWarning("There isn't any server connected to web!");
            } else
            {
                server = ConnectedServers.FirstOrDefault(s => s.Id == id);
                if (server == null)
                    logger.LogWarning($"Failed to find server with ID {id}!");
            }
            return server;
        }

        public void ToggleConnectedServers(IEnumerable<Shared.Server> servers)
        {
            foreach (var server in servers)
            {
                if (ConnectedServers.Any(x => x.Id == server.Id))
                    server.IsConnected = true;
                else
                    server.IsConnected = false;
            }
        }
    }
}
