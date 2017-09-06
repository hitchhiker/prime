using System;
using System.Collections.Generic;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class WalletAddresses : UniqueList<WalletAddress>
    {
        public WalletAddresses() { }

        public WalletAddresses(IEnumerable<WalletAddress> addresses) : base(addresses) { }

        public WalletAddresses(WalletAddress addr)
        {
            base.Add(addr);
        }

        public ObjectId NetworkId { get; set; }
    }
}