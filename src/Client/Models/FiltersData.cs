using Marketplace.Client.Models.Filters;
using Marketplace.Shared.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Marketplace.Client.Models
{
    public class FiltersData<TData> : IFilters<TData>
    {
        private readonly ICollection<TData> originData;
        public int PagesDefault { get; set; }
        public bool UseSearch { get; }
        public FiltersData(ICollection<TData> data, int pagesDefault = 0, bool useSearch = true, params IFilter[] filters)
        {
            originData = data.ToList();
            PagesDefault = pagesDefault;
            UseSearch = useSearch;
            DataCount = originData.Count;
            foreach (var filter in filters)
            {
                AttachFilter(filter);
            }
        }

        private string previousSearchString = string.Empty;
        public string SearchString { get; set; } = string.Empty;
        public int DataCount { get; set; }

        public IEnumerable<TData> FilteredData
        {
            get
            {
                List<TData> filterData = originData.ToList();

                if (UseSearch && previousSearchString != SearchString)
                {
                    previousSearchString = SearchString;
                    ApplySearch(filterData);
                    CurrentPage = 1;
                }

                ExecuteToggleFilters(filterData);
                if (CurrentOrderFilter != null) 
                    CurrentOrderFilter.Execute(ref filterData);              

                if (PagesDefault != 0)
                {
                    UpdatePagesState(filterData);
                    ApplyPagination(ref filterData);
                }

                DataCount = filterData.Count;                
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
            PagesCount = data.Count / PagesDefault + (data.Count % PagesDefault > 0 ? 1 : 0);
            CanGoPrev = CurrentPage > 1;
            CanGoNext = CurrentPage < PagesCount;
        }

        private void ApplyPagination(ref List<TData> data)
        {
            data = data.Skip((CurrentPage - 1) * PagesDefault).Take(PagesDefault).ToList();
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
            {
                if (filter.Enabled)
                {
                    if (CurrentOrderFilter == null)
                        CurrentOrderFilter = filter as IOrderFilter<TData>;
                    else
                        filter.Enabled = false;
                }
                OrderFilters.Add(filter as IOrderFilter<TData>);
            }                
        }

        private void ExecuteToggleFilters(List<TData> data)
        {
            foreach (var filter in ToggleFilters)
            {
                if (filter.Enabled)
                {
                    filter.Execute(data);
                }                    
            }
        }

        public IOrderFilter<TData> CurrentOrderFilter { get; private set; }        

        public void ChangeOrderFilter(IOrderFilter<TData> filter)
        {
            CurrentPage = 1;
            if (CurrentOrderFilter != null)
                CurrentOrderFilter.Enabled = false;
            CurrentOrderFilter = filter;
            CurrentOrderFilter.Enabled = true;
        }

        public void ToggleFilter(IToggleFilter<TData> filter)
        {
            filter.Enabled = !filter.Enabled;
            CurrentPage = 1;
        }

        public void RemoveFromOrigin(TData item)
        {
            originData.Remove(item);
        }

        public void AddToOrigin(TData item)
        {
            originData.Add(item);
        }
    }
}