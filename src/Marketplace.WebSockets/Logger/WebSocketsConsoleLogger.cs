using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.WebSockets.Logger
{
    public class WebSocketsConsoleLogger : IWebSocketsLogger
    {
        private readonly bool IsDebug;
        const string prefix = "WebSocketsConsoleLogger >> ";

        public WebSocketsConsoleLogger(bool isDebug = false)
        {
            IsDebug = isDebug;
        }

        public Task Log(string msg)
        {
            Console.WriteLine(prefix + msg);
            return Task.CompletedTask;
        }

        public Task LogDebug(string msg)
        {
            if (IsDebug)
                Console.WriteLine(prefix + msg);
            return Task.CompletedTask;
        }

        public Task LogError(Exception e, string msg)
        {
            Console.WriteLine(prefix + msg);
            Console.WriteLine(e.Message);
            return Task.CompletedTask;
        }
    }
}
