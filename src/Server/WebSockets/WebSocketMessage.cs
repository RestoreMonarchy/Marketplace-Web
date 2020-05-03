using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Marketplace.Server.WebSockets
{
    public class WebSocketMessage
    {
        public WebSocketMessage(WebSocket webSocket, string msg)
        {
            WebSocket = webSocket;
            Content = msg;
            Claimed = false;
        }

        public WebSocket WebSocket { get; set; }
        public string Content { get; set; }
        public bool Claimed { get; set; }

        public void Claim()
        {
            Claimed = true;
        }
    }
}
