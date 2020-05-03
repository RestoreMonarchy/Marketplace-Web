using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Marketplace.Server.WebSockets
{
    public class WebSocketsUtility
    {
        private readonly ILogger<WebSocketsUtility> logger;

        public WebSocketsUtility(ILogger<WebSocketsUtility> logger)
        {
            this.logger = logger;
        }

        private delegate Task OnMessageReceived(WebSocketMessage msg);
        private event OnMessageReceived onMessageReceived;

        public async Task ListenWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[256];
            var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                var msg = Encoding.ASCII.GetString(buffer);
                await onMessageReceived.Invoke(new WebSocketMessage(webSocket, msg));
            }
            await webSocket.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
        }

        public async Task<string> AskWebSocketAsync(WebSocket webSocket, string method, params object[] args)
        {
            string returnMsg = null;
            var receiveMethod = "tell" + method;

            var msg = "ask" + method + " " + string.Join(' ', args);
            var sendBuffer = Encoding.ASCII.GetBytes(msg);

            var signal = new SemaphoreSlim(0, 1);
            onMessageReceived += waitMessage;
            Task waitMessage(WebSocketMessage wsMsg)
            {
                if (wsMsg.WebSocket == webSocket && wsMsg.Content.StartsWith(receiveMethod))
                    return Task.CompletedTask;
                returnMsg = wsMsg.Content;
                wsMsg.Claim();                
                signal.Release();
                onMessageReceived -= waitMessage;
                return Task.CompletedTask;
            }

            await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Binary, 
                false, CancellationToken.None);
            await signal.WaitAsync();
            return returnMsg;
        }

        public async Task TellWebSocketAsync(WebSocket webSocket, string msg)
        {
            var buffer = Encoding.ASCII.GetBytes(msg);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary,
                false, CancellationToken.None);
        }
    }
}
