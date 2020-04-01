using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared
{
    public class ProductTransactions
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ExecuteCommands { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Product Product { get; set; }
    }
}
