using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface IServersRepository
    {
        Task<IEnumerable<Server>> GetServersAsync();
        Task<Server> GetServerAsync(int serverId);
        Task<int> AddServerAsync(Server server);
        Task ToggleServerAsync(int serverId);
        Task DeleteServerAsync(int serverId);
        Task UpdateServerAsync(Server server);
    }
}
