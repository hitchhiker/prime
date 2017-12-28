using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bit2c
{
    internal class Bit2CSchema
    {
        internal class TickerResponse
        {
            public decimal h;
            public decimal l;
            public decimal ll;
            public decimal a;
            public decimal av;
        }

        internal class OrderBookResponse
        {
            public object[][] bids;
            public object[][] asks;
        }
    }
}
