using Marketplace.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared
{
    public class ProductTransaction
    {
        public int Id { get; set; }
        [Searchable]
        public int ProductId { get; set; }
        public int ServerId { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool IsComplete { get; set; }
        public DateTime CreateDate { get; set; }

        [Searchable]
        public string ProductName => Product?.Title ?? string.Empty;
        [Searchable]
        public string ServerName => Server?.ServerName ?? string.Empty;

        public virtual Product Product { get; set; }
        public virtual Server Server { get; set; }
    }
}
