using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Settings
{
    public interface ISettingWatcher
    {
        string Name { get; }

        Task UpdatedAsync(string previousValue, string newValue);
    }
}
