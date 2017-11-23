using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BtcMarkets
{
    internal class BtcMarketsSchema
    {
        internal class TickerResponse
        {
            public decimal lastPrice;
            public decimal volume24h;
            public decimal bestBid;
            public decimal bestAsk;
            public long timestamp;
        }
    }
}
