using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BitBay
{
    internal class BitBaySchema
    {
        internal class TickerResponse
        {
            public decimal max;
            public decimal min;
            public decimal last;
            public decimal bid;
            public decimal ask;
            public decimal vwap;
            public decimal average;
            public decimal volume;
        }
    }
}
