using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.BitStamp
{
    internal class BitStampSchema
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
            public decimal open;
        }
    }
}
