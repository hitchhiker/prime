using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Exx
{
    internal class ExxSchema
    {
        internal class TickerResponse
        {
            public TickerEntryResponse ticker;
            public long date;
        }

        internal class AllTickersResponse : Dictionary<string, TickerEntryResponse>
        {
        }

        internal class TickerEntryResponse
        {
            public float vol;
            public decimal last;
            public decimal sell;
            public decimal buy;
            public decimal weekRiseRate;
            public decimal riseRate;
            public decimal high;
            public decimal low;
            public decimal monthRiseRate;
        }

        internal class OrderBookResponse
        {
            public long timestamp;
            public decimal[][] asks;
            public decimal[][] bids;
        }
    }
}
