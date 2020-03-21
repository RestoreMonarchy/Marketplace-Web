using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Models.Filters
{
    public interface IToggleFilter<TData> : IFilter
    {
        void Execute(List<TData> data);
    }
}
