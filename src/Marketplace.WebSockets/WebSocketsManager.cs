using Marketplace.WebSockets.Attributes;
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
    public class WebSocketsManager
    {
        public const int BufferSize = 1024 * 4;
        private uint id = 0;

        public delegate Task OnMessageReceived(WebSocketMessage msg);
        public event OnMessageReceived onMessageReceived;

        private List<WebSocketMessage> questionMessages = new List<WebSocketMessage>();

        private MethodInfo[] MethodCalls { get; set; }
        private object CallsInstance { get; set; }

        public void Initialize(Assembly assembly, object instance)
        {
            MethodCalls = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes<WebSocketCallAttribute>(false).Count() > 0)
                .ToArray();
            CallsInstance = instance;
        }

        public async Task ListenWebSocketAsync(WebSocket webSocket)
        {
            var buffer = new byte[BufferSize];
            WebSocketReceiveResult result;
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                await ProcessMessageAsync(webSocket, buffer);
            } while (!result.CloseStatus.HasValue);
            await webSocket.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
        }

        private async Task ProcessMessageAsync(WebSocket webSocket, byte[] buffer)
        {
            var msg = WebSocketMessage.FromJson(buffer);
            msg.WebSocket = webSocket;
            Console.WriteLine($"received message: {msg.Method}");

            var waitingMessage = questionMessages.FirstOrDefault(x => x.Id == msg.QuestionId);
            if (waitingMessage != null)
                waitingMessage.Respond(msg);
            else
                await ReceiveMessage(msg);
        }

        public async Task ReceiveMessage(WebSocketMessage msg)
        {
            //if (onMessageReceived != null)
            //    await onMessageReceived.Invoke(msg);
            if (MethodCalls != null)
            {        
                var method = MethodCalls.SingleOrDefault(m => m.GetCustomAttribute<WebSocketCallAttribute>().Name == msg.Method);
                if (method != null)
                {
                    try
                    {
                        await (Task)method.Invoke(CallsInstance, new object[] { msg });
                    } catch (Exception e)
                    {
                        // TODO: implement WebSocketsLogger
                        Console.WriteLine(e);
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
            CancellationTokenSource tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3)); //Put in settins file

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
                Arguments = args,
                IsQuestion = false,
                QuestionId = questionId
            };
            await SendWebSocketAsync(webSocket, msg);
        }

        private async Task SendWebSocketAsync(WebSocket webSocket, WebSocketMessage msg, CancellationToken cancellationToken = default)
        {
            var buffer = Encoding.ASCII.GetBytes(msg.GetJson());
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, cancellationToken);
        }
    }
}