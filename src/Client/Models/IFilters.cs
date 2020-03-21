using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Models
{
    public interface IFilters
    {
        int CurrentPage { get; set; }
        string SearchString { get; set; }
        bool CanGoPrev { get; }
        bool CanGoNext { get; }
        void PrevPage();
        void NextPage();
    }
}
