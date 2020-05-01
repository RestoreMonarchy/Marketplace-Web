using Marketplace.DatabaseProvider.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public class ServersService
    {
        private readonly IServersRepository serversRepository;
        private readonly ILogger<ServersService> logger;

        private List<Shared.Server> servers = new List<Shared.Server>();
        public IEnumerable<Shared.Server> Servers => servers;
        public Dictionary<int, List<string>> MessagesQueue { get; set; } = new Dictionary<int, List<string>>();

        public ServersService(IServersRepository serversRepository, ILogger<ServersService> logger)
        {
            this.serversRepository = serversRepository;
            this.logger = logger;
        }

        public async Task ListenWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            var server = await GetServerAsync(webSocket);
            if (server == null)
            {
                logger.LogInformation("Server didnt respond id or not found");
                return;
            }
            Console.WriteLine("ServerName " + server.ServerName);
            logger.LogInformation($"Server Name: {server.ServerName}");
            server.WebSocket = webSocket;

            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var msgs = MessagesQueue[server.Id];
                if (msgs.Count > 0)
                {   
                    buffer = Encoding.ASCII.GetBytes(msgs[0]);
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, 
                        false, CancellationToken.None);
                    msgs.RemoveAt(0);
                }

                var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.Count > 0)
                {
                    var content = Encoding.ASCII.GetString(buffer);
                    logger.LogInformation($"received msg from server: {content}");
                }
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
            server.WebSocket = null;
        }

        public async Task<Shared.Server> GetServerAsync(WebSocket webSocket)
        {
            var msgBuffer = Encoding.ASCII.GetBytes("askServerId");
            await webSocket.SendAsync(new ArraySegment<byte>(msgBuffer), WebSocketMessageType.Binary, false, CancellationToken.None);
            var buffer = new byte[1024 * 4];
            string receivedMsg;
            while (webSocket.State == WebSocketState.Open)
            {
                await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                receivedMsg = Encoding.ASCII.GetString(buffer);
                if (!string.IsNullOrEmpty(receivedMsg))
                {
                    Console.WriteLine("got message");
                    var arr = receivedMsg.Split(' ');
                    if (arr[0] == "tellServerId" && int.TryParse(arr[1], out var num))
                    {
                        return await serversRepository.GetServerAsync(num);
                    }
                }
            }
            return null;
        }
    }
}
