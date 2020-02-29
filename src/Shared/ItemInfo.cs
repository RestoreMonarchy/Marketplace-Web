using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared
{
    public class ItemInfo
    {
        public ushort ItemId { get; set; }
        public byte Quality { get; set; }
        public byte Amount { get; set; }
        public byte[] Metadata { get; set; }

        public static ItemInfo Create(ushort id, byte quality, byte amount, byte[] metadata)
        {
            return new ItemInfo()
            {
                ItemId = id,
                Quality = quality,
                Amount = amount,
                Metadata = metadata
            };
        }
    }
}
