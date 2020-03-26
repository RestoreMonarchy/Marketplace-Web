using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Models.Filters
{
    public interface IOrderFilter<TData> : IFilter
    {
        void Execute(ref List<TData> data);
    }
}
