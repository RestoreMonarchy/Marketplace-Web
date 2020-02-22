using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Models
{
    public class IconCache
    {
        public IconCache(byte[] data, DateTime lastUpdate)
        {
            Data = data;
            LastUpdate = lastUpdate;
        }
        public byte[] Data { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
