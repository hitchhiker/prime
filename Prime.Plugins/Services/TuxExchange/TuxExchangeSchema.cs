using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.TuxExchange
{
    internal class TuxEchangeSchema
    {
        internal class TickerResponse
        {
            public int id;
            public int isFrozen;
            public decimal last;
            public decimal lowestAsk;
            public decimal highestBid;
            public decimal percentChange;
            public decimal quoteVolume;
            public decimal baseVolume;
            public decimal high24hr;
            public decimal low24hr;
        }

        internal class AllTickersResponse : Dictionary<string, TuxEchangeSchema.TickerResponse>
        {
        }

        internal class VolumeResponse : Dictionary<string, Dictionary<string, decimal>>
        {
        }
    }
}
