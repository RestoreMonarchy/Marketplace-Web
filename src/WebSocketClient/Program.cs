using Marketplace.WebSockets;
using Marketplace.WebSockets.Attributes;
using Marketplace.WebSockets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var instance = new Program();
            instance.StartAsync().Wait();
            Console.ReadKey();
        }

        public WebSocketsManager manager;
        public ClientWebSocket client;

        public async Task StartAsync()
        {
            manager = new WebSocketsManager();
            manager.Initialize(GetType().Assembly, this);
            client = new ClientWebSocket();
            await client.ConnectAsync(new Uri("ws://localhost:5000/ws"), CancellationToken.None);
            await manager.TellWebSocketAsync(client, "ServerId", null, 1);
            await manager.ListenWebSocketAsync(client);

        }

        [WebSocketCall("PlayerBalance")]
        public async Task TellPlayerBalanceAsync(WebSocketMessage question)
        {
            Console.WriteLine("TELL PLAYER BALANCE");
            if ((string)question.Arguments[0] == "76561198285897058")
            {
                await manager.TellWebSocketAsync(client, "PlayerBalance", question.Id, 125);
            } else
            {
                await manager.TellWebSocketAsync(client, "PlayerBalance", question.Id, 20);
            }
        }
    }
}
