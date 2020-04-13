using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared
{
    public class ServerTransaction
    {
        public int TransactionId { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public DateTime TransactionCreateDate { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductPrice { get; set; }

        public virtual List <Command> Commands { get; set; }
        public class Command
        {
            public int Id { get; set; }
            public string CommandName { get; set; }
            public string CommandText { get; set; }
            public bool Expires { get; set; }
            public int ExpireTime { get; set; }
            public string ExpireCommand { get; set; }
            public bool ExecuteOnBuyerJoinServer { get; set; }
        }
    }
}
