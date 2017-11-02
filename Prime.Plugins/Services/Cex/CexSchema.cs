using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Cex
{
    internal class CexSchema
    {
        internal class TickersResponse
        {
            public string e;
            public string ok;
            public TickerResponse[] data;
        }

        internal class TickerResponse
        {
            public long timestamp;
            public string pair;
            public decimal low;
            public decimal high;
            public decimal last;
            public decimal volume;
            public decimal volume30d;
            public decimal bid;
            public decimal ask;
        }
    }
}
