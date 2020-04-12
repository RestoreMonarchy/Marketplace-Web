using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared
{
    public class ProductTransaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ServerId { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool IsFinished { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Product Product { get; set; }
        public virtual Server Server { get; set; }
    }
}
