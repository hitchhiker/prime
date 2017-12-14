using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bitlish
{
    internal class BitlishSchema
    {
        internal class AllTickersResponse : Dictionary<string, TickerResponse>
        {
        }

        internal class TickerResponse
        {
            public decimal first;
            public decimal last;
            public decimal max;
            public decimal min;
            public decimal prc;
            public decimal sum;
        }
    }
}
