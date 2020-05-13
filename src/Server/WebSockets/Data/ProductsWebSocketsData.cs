using Marketplace.Server.Services;
using Marketplace.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Marketplace.Server.WebSockets.Data
{
    public class ProductsWebSocketsData : IProductsWebSocketsData
    {
        private readonly IWebSocketsManager webSocketsManager;
        private readonly IServersService serversService;

        public ProductsWebSocketsData(IWebSocketsManager webSocketsManager, IServersService serversService)
        {
            this.webSocketsManager = webSocketsManager;
            this.serversService = serversService;
        }

        public async Task ProductTransactionAsync(int transactionId, int serverId)
        {
            var server = serversService.GetConnectedServer(serverId);
            if (server == null)
                return;

            await webSocketsManager.TellWebSocketAsync(server.WebSocket, "ProductTransaction", null, transactionId);
        }
    }
}
