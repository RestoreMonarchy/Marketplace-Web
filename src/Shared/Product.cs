using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ExecuteCommands { get; set; }
        public byte[] Icon { get; set; }
        public int MaxPurchases { get; set; }
        public bool Expires { get; set; }
        public bool Enabled { get; set; }

        public virtual IEnumerable<Server> Servers { get; set; }
        public virtual IEnumerable<Command> Commands { get; set; }
    }
}
