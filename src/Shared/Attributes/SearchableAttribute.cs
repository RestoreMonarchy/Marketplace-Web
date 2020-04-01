using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class SearchableAttribute : Attribute
    {
        public bool IgnoreCase { get; set; }
        public SearchableAttribute(bool ignoreCase = true)
        {
            IgnoreCase = ignoreCase;
        }
    }
}
