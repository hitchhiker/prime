using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.ItBit
{
    internal class ItBitSchema
    {
        internal class TickerResponse
        {
            public string pair;
            public decimal bid;
            public decimal bidAmt;
            public decimal ask;
            public decimal askAmt;
            public decimal lastPrice;
            public decimal lastAmt;
            public decimal volume24h;
            public decimal volumeToday;
            public decimal high24h;
            public decimal low24h;
            public decimal highToday;
            public decimal lowToday;
            public decimal openToday;
            public decimal vwapToday;
            public decimal vwap24h;
            public DateTime serverTimeUTC;
        }
    }
}
