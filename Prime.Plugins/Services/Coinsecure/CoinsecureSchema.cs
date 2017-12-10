using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Coinsecure
{
    internal class CoinsecureSchema
    {
        internal class TickerResponse
        {
            public bool success;
            public long time;
            public TickerEntryResponse message;
        }

        internal class TickerEntryResponse
        {
            public long timestamp;
            public decimal lastPrice;
            public decimal bid;
            public decimal ask;
            public decimal fiatvolume;
            public decimal coinvolume;
            public decimal open;
            public decimal high;
            public decimal low;
        }
    }
}
