using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.WebSockets.Logger
{
    public interface IWebSocketsLogger
    {
        Task Log(string msg);
        Task LogDebug(string msg);
        Task LogError(Exception e, string msg);
    }
}
