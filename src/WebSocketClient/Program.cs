using Marketplace.WebSockets;
using Marketplace.WebSockets.Attributes;
using Marketplace.WebSockets.Logger;
using Marketplace.WebSockets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
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
        public IWebSocketsLogger logger;

        public async Task StartAsync()
        {
            logger = new WebSocketsConsoleLogger(true);
            manager = new WebSocketsManager(logger);
            manager.Initialize(GetType().Assembly, new object[] { this });
            client = new ClientWebSocket();
            await client.ConnectAsync(new Uri("ws://localhost:5000/ws"), CancellationToken.None);

            // Tell which server has connected
            await manager.TellWebSocketAsync(client, "ServerId", null, 1);
            await manager.ListenWebSocketAsync(client);
        }

        public Dictionary<string, decimal> PlayerBalances { get; set; } = new Dictionary<string, decimal>() 
        {
            //{ "76561198285897058", 100 }
        };

        [WebSocketCall("PlayerBalance")]
        private async Task TellPlayerBalanceAsync(WebSocketMessage question)
        {
            var playerId = (string)question.Arguments[0];
            if (PlayerBalances.TryGetValue(playerId, out decimal balance))
            {
                await manager.TellWebSocketAsync(client, "PlayerBalance", question.Id, balance);
            } else
            {
                PlayerBalances.Add(playerId, 30);
                await manager.TellWebSocketAsync(client, "PlayerBalance", question.Id, 30);
            }
        }

        [WebSocketCall("IncrementPlayerBalance")]
        private async Task TellIncrementBalanceAsync(WebSocketMessage question)
        {
            var playerId = Convert.ToString(question.Arguments[0]);
            var amount = Convert.ToDecimal(question.Arguments[1]);
            decimal postBalance;
            if (PlayerBalances.TryGetValue(playerId, out var balance))
            {
                postBalance = balance + amount;
                if (postBalance >= 0)
                {
                    PlayerBalances[playerId] = postBalance;
                    await manager.TellWebSocketAsync(client, "IncrementPlayerBalance", question.Id, true);
                    return;
                }                
            } else
            {
                postBalance = 20 + amount;
                if (postBalance >= 0)
                {
                    PlayerBalances.Add(playerId, postBalance);
                    await manager.TellWebSocketAsync(client, "IncrementPlayerBalance", question.Id, true);
                    return;
                }                
            }
            await manager.TellWebSocketAsync(client, "IncrementPlayerBalance", question.Id, false);
        }

        [WebSocketCall("Pay")]
        private async Task PayAsync(WebSocketMessage question)
        {
            var payerId = Convert.ToString(question.Arguments[0]);
            var receiverId = Convert.ToString(question.Arguments[1]);
            var amount = Convert.ToDecimal(question.Arguments[2]);

            InitializePlayerBalance(payerId);
            InitializePlayerBalance(receiverId);

            if (PlayerBalances[payerId] - amount >= 0)
            {
                await manager.TellWebSocketAsync(client, "IncrementPlayerBalance", question.Id, true);
                PlayerBalances[payerId] -= amount;
                PlayerBalances[receiverId] += amount;
            }
            await manager.TellWebSocketAsync(client, "IncrementPlayerBalance", question.Id, false);
        }

        private void InitializePlayerBalance(string playerId)
        {
            if (!PlayerBalances.ContainsKey(playerId))
                PlayerBalances.Add(playerId, 30);
        }
    }
}
