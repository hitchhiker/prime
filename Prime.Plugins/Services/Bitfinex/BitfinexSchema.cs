using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bitfinex
{
    internal class BitfinexSchema
    {
        internal class TickerResponse
        {
            public decimal mid;
            public decimal bid;
            public decimal ask;
            public decimal last_price;
            public decimal low;
            public decimal high;
            public decimal volume;
            public decimal timestamp;
        }
    }
}
