using Marketplace.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface IServersRepository
    {
        Task<IEnumerable<Server>> GetServersAsync();
        Task<Server> GetServerAsync(int serverId);
        Task<int> CreateServerAsync(Server server);
        Task UpdateServerAsync(Server server);
    }
}
