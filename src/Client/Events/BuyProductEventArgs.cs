using Marketplace.Shared;
using System;

namespace Marketplace.Client.Events
{
    public class BuyProductEventArgs : EventArgs
    {
        public BuyProductEventArgs(Product product, Server selectedServer)
        {
            Product = product;
            SelectedServer = selectedServer;
        }
        public Product Product { get; set; }
        public Server SelectedServer { get; set; }
    }
}
