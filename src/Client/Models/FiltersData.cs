using Marketplace.Client.Models.Filters;
using Marketplace.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Marketplace.Client.Models
{
    public class FiltersData<TData> : IFilters<TData>
    {
        private readonly IEnumerable<TData> originData;
        private readonly int pagesDefault;
        public FiltersData(IEnumerable<TData> data, int pagesDefault = 0, params IFilter[] filters)
        {
            this.originData = data;
            this.pagesDefault = pagesDefault;
            foreach (var filter in filters)
            {
                AttachFilter(filter);
            }
        }

        public string SearchString { get; set; } = string.Empty;        

        public IEnumerable<TData> FilteredData
        {
            get
            {
                List<TData> filterData = originData.ToList();

                ApplySearch(filterData);

                ExecuteToggleFilters(filterData);
                if (CurrentOrderFilter != null)
                    CurrentOrderFilter.Execute(ref filterData);

                if (pagesDefault != 0)
                {
                    UpdatePagesState(filterData);
                    ApplyPagination(ref filterData);
                }
                return filterData;
            }
        }

        private void ApplySearch(List<TData> data)
        {            
            if (string.IsNullOrEmpty(SearchString))
                return;

            var properties = typeof(TData).GetProperties().Where(x => x.GetCustomAttribute<SearchableAttribute>() != null)
                .ToDictionary(x => x, x => x.GetCustomAttribute<SearchableAttribute>());

            for (int i = 0; i < data.Count; i++)
            {
                bool isRemove = true;
                foreach (var property in properties)
                {                    
                    var value = property.Key.GetValue(data[i]).ToString();
                    if (!property.Value.IgnoreCase && value.Contains(SearchString)
                        || property.Value.IgnoreCase && value.ToLower().Contains(SearchString.ToLower()))
                    {
                        isRemove = false;
                        break;
                    }
                }

                if (isRemove)
                {
                    data.RemoveAt(i);
                    i--;
                }
            }
        }

        public int CurrentPage { get; set; } = 1;
        public int PagesCount { get; set; }

        private void UpdatePagesState(List<TData> data)
        {
            PagesCount = data.Count / pagesDefault + (data.Count % pagesDefault > 0 ? 1 : 0);
            CanGoPrev = CurrentPage > 1;
            CanGoNext = CurrentPage < PagesCount;
        }

        private void ApplyPagination(ref List<TData> data)
        {
            data = data.Skip((CurrentPage - 1) * pagesDefault).Take(pagesDefault).ToList();
        }

        public bool CanGoPrev { get; set; }
        public bool CanGoNext { get; set; }

        public void PrevPage()
        {
            if (CanGoPrev)
                CurrentPage--;
        }

        public void NextPage()
        {
            if (CanGoNext)
                CurrentPage++;
        }

        IEnumerable<IToggleFilter<TData>> IFilters<TData>.ToggleFilters => ToggleFilters;
        IEnumerable<IOrderFilter<TData>> IFilters<TData>.OrderFilters => OrderFilters;

        public List<IToggleFilter<TData>> ToggleFilters { get; } = new List<IToggleFilter<TData>>();
        public List<IOrderFilter<TData>> OrderFilters { get; } = new List<IOrderFilter<TData>>();

        public void AttachFilter(IFilter filter)
        {
            if (filter as IToggleFilter<TData> != null)
                ToggleFilters.Add(filter as IToggleFilter<TData>);

            if (filter as IOrderFilter<TData> != null)
                OrderFilters.Add(filter as IOrderFilter<TData>);
        }

        public void AttachFilter(IOrderFilter<TData> filter)
        {
            OrderFilters.Add(filter);
        }

        private void ExecuteToggleFilters(List<TData> data)
        {
            foreach (var filter in ToggleFilters)
            {
                if (filter.Enabled)
                    filter.Execute(data);
            }
        }

        public IOrderFilter<TData> CurrentOrderFilter { get; private set; }        

        public void ChangeOrderFilter(IOrderFilter<TData> filter)
        {
            CurrentOrderFilter = filter;
        }
    }
}