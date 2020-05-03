using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class WebSocketCallAttribute : Attribute
    {
        public WebSocketCallAttribute()
        {
        }
    }
}
