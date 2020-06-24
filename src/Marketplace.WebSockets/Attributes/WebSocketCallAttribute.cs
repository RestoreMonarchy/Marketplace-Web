using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.WebSockets.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class WebSocketCallAttribute : Attribute
    {
        public string Name { get; set; }
        public WebSocketCallAttribute(string name)
        {
            Name = name;
        }
    }
}
