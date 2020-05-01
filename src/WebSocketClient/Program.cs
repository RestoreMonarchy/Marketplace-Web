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
        
        public async Task StartAsync()
        {
            ClientWebSocket client = null;
            try
            {
                client = new ClientWebSocket();
                await client.ConnectAsync(new Uri("ws://localhost:5000/ws"), CancellationToken.None);
                if (client.State == WebSocketState.Open)
                {
                    Console.WriteLine("Connected to web");
                }
                await Task.WhenAll(ReceiveAsync(client));
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task ReceiveAsync(ClientWebSocket client)
        {
            var buffer = new byte[4 * 1024];
            while (client.State == WebSocketState.Open)
            {
                await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var msg = Encoding.ASCII.GetString(buffer);
                if (msg.Length == 0)
                    continue;
                Console.WriteLine($"received msg from server: {msg}");
                if (msg.Contains("askServerId"))
                {
                    Console.WriteLine("sending server ID");
                    await SendAsync(client, "tellServerId 1");
                }
            }
        }

        public async Task SendAsync(ClientWebSocket client, string msg)
        {
            var buffer = Encoding.ASCII.GetBytes(msg);
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
        }
    }
}
