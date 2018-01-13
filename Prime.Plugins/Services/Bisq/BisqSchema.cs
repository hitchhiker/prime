using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bisq
{
    internal class BisqSchema
    {
        internal class AllTickersResponse : Dictionary<string, TickerEntryResponse> { }

        internal class OrderBookResponse : Dictionary<string, OrderBookEntryResponse> { }

        internal class TickerResponse : List<TickerEntryResponse> { }

        internal class TickerEntryResponse
        {
            public decimal? last;
            public decimal? high;
            public decimal? low;
            public decimal? volume_left;
            public decimal? volume_right;
            public decimal? buy;
            public decimal? sell;
        }

        internal class OrderBookEntryResponse
        {
            public decimal[] buys;
            public decimal[] sells;
        }
    }
}
