using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.OkCoin
{
    internal class OkCoinSchema
    {
        internal class TickerResponse
        {
            public long date;
            public TickerEntryResponse ticker;
        }

        internal class TickerEntryResponse
        {
            public decimal buy;
            public decimal high;
            public decimal last;
            public decimal low;
            public decimal sell;
            public decimal vol;
        }
    }
}
