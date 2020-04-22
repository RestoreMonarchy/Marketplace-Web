using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Settings
{
    public class EconomyTypeSettingWatcher : ISettingWatcher
    {
        public string Name => "EconomyProvider";

        public Task UpdatedAsync(string previousValue, string newValue)
        {
            return Task.CompletedTask;
            //TODO: Replace previous provider with new.
        }
    }
}
