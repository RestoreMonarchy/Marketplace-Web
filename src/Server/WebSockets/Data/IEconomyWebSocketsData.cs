using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.WebSockets.Data
{
    public interface IEconomyWebSocketsData 
    {
        Task<decimal?> GetPlayerBalanceAsync(string playerId);
        Task<bool> PayAsync(string senderId, string receiverId, decimal amount);
        Task<bool?> IncrementBalanceAsync(string playerId, decimal amount);
    }
}