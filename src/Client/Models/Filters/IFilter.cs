using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Models
{
    public interface IFilter
    {
        string Text { get; }
        bool Enabled { get; }        
        void Toggle();
    }
}
