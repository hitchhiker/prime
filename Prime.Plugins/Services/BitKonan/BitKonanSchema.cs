using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BitKonan
{
    internal class BitKonanSchema
    {
        internal class TickerResponse
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal bid;
            public decimal ask;
            public decimal open;
            public decimal volume;
        }

        internal class OrderBookResponse
        {
            public Dictionary<string, decimal>[] bid;
            public Dictionary<string, decimal>[] ask;
        }
    }
}
