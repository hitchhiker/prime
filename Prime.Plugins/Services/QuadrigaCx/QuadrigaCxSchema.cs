using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.QuadrigaCx
{
    internal class QuadrigaCxSchema
    {
        internal class TickerResponse
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal vwap;
            public decimal volume;
            public decimal bid;
            public decimal ask;
            public long timestamp;
        }
    }
}
