using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Luno
{
    internal class LunoSchema
    {
        internal class AllTickersResponse
        {
            public TickerResponse[] tickers;
        }

        internal class TickerResponse
        {
            public string pair;
            public decimal bid;
            public decimal ask;
            public decimal last_trade;
            public decimal rolling_24_hour_volume;
            public long timestamp;
        }
    }
}
