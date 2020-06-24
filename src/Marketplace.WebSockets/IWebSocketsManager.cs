using Marketplace.WebSockets.Models;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;

namespace Marketplace.WebSockets
{
    public interface IWebSocketsManager
    {
        void Initialize(Assembly assembly, object[] instances);
        Task ListenWebSocketAsync(WebSocket webSocket);
        Task<WebSocketMessage> AskWebSocketAsync(WebSocket webSocket, string method, params string[] args);
        Task TellWebSocketAsync(WebSocket webSocket, string method, uint? questionId, params object[] args);
    }
}