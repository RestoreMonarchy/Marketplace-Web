using Marketplace.Server.WebSockets;
using Marketplace.Shared;
using Marketplace.WebSockets.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public interface IServersService : IWebSocketCaller
    {
        IEnumerable<Shared.Server> ConnectedServers { get; }
        Task ListenServerWebSocketAsync(HttpContext context, WebSocket webSocket);
        Shared.Server GetConnectedServer(int? id = null);
        void ToggleConnectedServers(IEnumerable<Shared.Server> servers);
    }
}