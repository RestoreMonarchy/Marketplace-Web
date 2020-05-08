﻿using Marketplace.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories
{
    public interface ICommandsRepository : IRepository
    {
        Task<IEnumerable<Command>> GetCommandsAsync();
        Task<int> AddCommandAsync(Command command);
        Task UpdateCommandAsync(Command command);
    }
}
