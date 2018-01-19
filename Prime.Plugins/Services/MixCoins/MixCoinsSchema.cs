using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.MixCoins
{
    internal class MixCoinsSchema
    {
        internal class TickerResponse
        {
            public int status;
            public string message;
            public TickerEntryResponse result;
        }

        internal class TickerEntryResponse
        {
            public decimal last;
            public decimal sell;
            public decimal buy;
            public decimal high;
            public decimal low;
            public decimal vol;
        }

        internal class OrderBookResponse
        {
            public int status;
            public string message;
            public OrderBookEntryResponse result;
        }

        internal class OrderBookEntryResponse
        {
            public decimal[][] asks;
            public decimal[][] bids;
        }
    }
}
