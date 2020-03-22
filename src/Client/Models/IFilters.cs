using Marketplace.Client.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Models
{
    public interface IFilters<TData>
    {
        int CurrentPage { get; set; }
        string SearchString { get; set; }
        bool CanGoPrev { get; }
        bool CanGoNext { get; }
        void PrevPage();
        void NextPage();
        int DataCount { get; }
        bool UseSearch { get; }

        IEnumerable<IToggleFilter<TData>> ToggleFilters { get; }
        IEnumerable<IOrderFilter<TData>> OrderFilters { get; }
        void ChangeOrderFilter(IOrderFilter<TData> filter);
        IOrderFilter<TData> CurrentOrderFilter { get; }
    }
}
