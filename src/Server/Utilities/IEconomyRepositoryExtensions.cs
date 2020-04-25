using Marketplace.DatabaseProvider.Repositories.MySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Utilities
{
    public static class IEconomyRepositoryExtensions
    {
        public static Type ParseEconomyProviderType(string name, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (name.Equals("AviEconomy", comparison))
                return typeof(AviEconomyRepository);
            if (name.Equals("Uconomy", comparison))
                return typeof(UconomyEconomyRepository);
            throw new ArgumentException($"Economy provider: {name} was not found!");
        }
    }
}
