using Marketplace.Client.Models.Filters;
using System.Collections.Generic;

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
        void RemoveFromOrigin(TData item);

        IEnumerable<IToggleFilter<TData>> ToggleFilters { get; }
        IEnumerable<IOrderFilter<TData>> OrderFilters { get; }
        void ChangeOrderFilter(IOrderFilter<TData> filter);
        void ToggleFilter(IToggleFilter<TData> filter);
        IOrderFilter<TData> CurrentOrderFilter { get; }
    }
}
