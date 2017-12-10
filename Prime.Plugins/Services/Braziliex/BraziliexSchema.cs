using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Braziliex
{
    internal class BraziliexSchema
    {
        internal class AllTickersResponse : Dictionary<string, TickerResponse>
        {
        }

        internal class TickerResponse
        {
            public int active;
            public string market;
            public decimal last;
            public decimal percentChange;
            public decimal baseVolume;
            public decimal quoteVolume;
            public decimal highestBid;
            public decimal lowestAsk;
            public decimal baseVolume24;
            public decimal quoteVolume24;
            public decimal highestBid24;
            public decimal lowestAsk24;
        }
    }
}
