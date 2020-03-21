using Marketplace.Shared;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Marketplace.Client.Models
{     
    public class FiltersData<TData> : IFilters
    {
        private readonly IEnumerable<TData> originData;
        private readonly int pagesDefault;
        public FiltersData(IEnumerable<TData> data, int pagesDefault = 0)
        {
            this.originData = data;
            this.pagesDefault = pagesDefault;
            //CacheValues();
        }

        public void CacheValues()
        {
            var data = originData.ToList();
            var properties = typeof(TData).GetProperties();

            for (int i = 0; i < data.Count; i++)
            {
                foreach (var property in properties)
                {
                    SearchableAttribute attribute;
                    if ((attribute = property.GetCustomAttribute<SearchableAttribute>()) != null)
                    {
                        var value = property.GetValue(data[i]).ToString();
                        if (!attribute.IgnoreCase && value.Contains(SearchString)
                            || attribute.IgnoreCase && value.ToLower().Contains(SearchString.ToLower()))
                        {

                        }
                    }
                }
            }
        }

        public string SearchString { get; set; } = string.Empty;

        public IEnumerable<TData> FilteredData
        {
            get
            {
                List<TData> filterData = originData.ToList();
                Stopwatch sw = new Stopwatch();
                sw.Start();

                ApplySearch(filterData);
                sw.Stop();
                System.Console.WriteLine("Search took: " + sw.ElapsedMilliseconds);
                    
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
            var peroperties = typeof(TData).GetProperties();
            if (string.IsNullOrEmpty(SearchString))
                return;

            for (int i = 0; i < data.Count; i++)
            {
                bool isRemove = true;
                foreach (var property in peroperties)
                {
                    SearchableAttribute attribute;
                    if ((attribute = property.GetCustomAttribute<SearchableAttribute>()) != null)
                    {
                        var value = property.GetValue(data[i]);
                        if (!attribute.IgnoreCase && value.ToString().Contains(SearchString)
                            || attribute.IgnoreCase && value.ToString().ToLower().Contains(SearchString.ToLower()))
                        {
                            isRemove = false;
                            break;
                        }
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
    }
}
