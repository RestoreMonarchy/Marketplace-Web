using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.WebSockets.Data
{
    public interface IProductsWebSocketsData : IWebSocketCaller
    {
        Task ProductTransactionAsync(int transactionId, int serverId);
    }
}