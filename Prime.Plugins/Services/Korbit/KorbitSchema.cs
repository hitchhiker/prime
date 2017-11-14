using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.Korbit
{
    internal class KorbitSchema
    {
        internal class TickerResponse
        {
            public long timestamp;
            public decimal last;
        }

        internal class DetailedTickerResponse : TickerResponse
        {
            public decimal bid;
            public decimal ask;
            public decimal low;
            public decimal high;
            public decimal volume;
        }

        internal class OrderBookResponse
        {
            public long timestamp;
            public decimal[][] bids;
            public decimal[][] asks;
        }
    }
}
