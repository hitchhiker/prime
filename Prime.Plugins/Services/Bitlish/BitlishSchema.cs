using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bitlish
{
    internal class BitlishSchema
    {
        internal class AllTickersResponse : Dictionary<string, TickerResponse>
        {
        }

        internal class TickerResponse
        {
            public decimal first;
            public decimal last;
            public decimal max;
            public decimal min;
            public decimal prc;
            public decimal sum;
        }

        internal class OrderBookEntryResponse
        {
            public decimal price;
            public decimal volume;
        }

        internal class OrderBookResponse
        {
            public decimal ask_end;
            public decimal bid_end;
            public long last;
            public string pair_id;
            public OrderBookEntryResponse[] bid;
            public OrderBookEntryResponse[] ask;
        }

    }
}
