using Marketplace.WebSockets.Attributes;
using Marketplace.WebSockets.Logger;
using Marketplace.WebSockets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Marketplace.WebSockets
{
    public class WebSocketsManager : IWebSocketsManager
    {
        public const int BufferSize = 1024 * 4;
        public const int TimeoutMiliseconds = 3000;
        private uint id = 0;

        private readonly IWebSocketsLogger logger;

        public delegate Task OnMessageReceived(WebSocketMessage msg);
        public event OnMessageReceived onMessageReceived;

        private List<WebSocketMessage> questionMessages = new List<WebSocketMessage>();
        
        private Assembly CallAssembly { get; set; }
        private MethodInfo[] MethodCalls { get; set; }
        private object[] CallInstances { get; set; }

        public WebSocketsManager(IWebSocketsLogger logger = null)
        {
            this.logger = logger;
        }

        public void Initialize(Assembly assembly, object[] instances)
        {
            CallAssembly = assembly;
            MethodCalls = assembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(m => m.GetCustomAttributes<WebSocketCallAttribute>(false).Count() > 0)
                .ToArray();

            CallInstances = instances;
        }

        public async Task ListenWebSocketAsync(WebSocket webSocket)
        {
            var buffer = new byte[BufferSize];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            do
            {
                await ProcessMessageAsync(webSocket, buffer, result.Count);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);                
            } while (!result.CloseStatus.HasValue);
            await webSocket.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
        }

        private async Task ProcessMessageAsync(WebSocket webSocket, byte[] buffer, int count)
        {
            var msg = WebSocketMessage.FromJson(buffer, count);
            msg.WebSocket = webSocket;

            if (logger != null)
                await logger.LogDebug($"Received message: {msg.Method}");

            var waitingMessage = questionMessages.FirstOrDefault(x => x.Id == msg.QuestionId);
            if (waitingMessage != null)
                waitingMessage.Respond(msg);
            else
                await ReceiveMessage(msg);
        }

        public async Task ReceiveMessage(WebSocketMessage msg)
        {
            if (onMessageReceived != null)
                await onMessageReceived(msg);

            if (MethodCalls != null)
            {
                var method = MethodCalls.SingleOrDefault(m => m.GetCustomAttribute<WebSocketCallAttribute>().Name == msg.Method);
                if (method != null)
                {
                    try
                    {
                        var instance = CallInstances.FirstOrDefault(c => 
                        {
                            return c.GetType() == method.DeclaringType;
                        });
                        await (Task)method.Invoke(instance, new object[] { msg });
                    }
                    catch (Exception e)
                    {
                        if (logger != null)
                            await logger.LogError(e, $"An error occurated when trying to invoke {method.Name} WebSocketCall");
                    }
                }
            }
        }

        public async Task<WebSocketMessage> AskWebSocketAsync(WebSocket webSocket, string method, params string[] args)
        {
            var msg = new WebSocketMessage()
            {
                Id = id++,
                Method = method,
                Arguments = args,
                IsQuestion = true,
                Signal = new SemaphoreSlim(0, 1)
            };
            questionMessages.Add(msg);
            var tokenSource = new CancellationTokenSource(TimeoutMiliseconds);

            try
            {
                await SendWebSocketAsync(webSocket, msg, tokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                msg.Signal.Release();
                throw new TimeoutException();
            }

            await msg.Signal.WaitAsync();
            return msg.Response;
        }

        public async Task TellWebSocketAsync(WebSocket webSocket, string method, uint? questionId, params object[] args)
        {
            var msg = new WebSocketMessage()
            {
                Id = id++,
                Method = method,
                Arguments = args.Select(x => x.ToString()).ToArray(),
                IsQuestion = false,
                QuestionId = questionId
            };
            await SendWebSocketAsync(webSocket, msg);
        }

        private async Task SendWebSocketAsync(WebSocket webSocket, WebSocketMessage msg, CancellationToken cancellationToken = default)
        {
            var buffer = Encoding.ASCII.GetBytes(msg.GetJson());
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
            if (logger != null)
                await logger.LogDebug($"Sent message {msg.Method}");
        }
    }
}