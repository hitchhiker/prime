using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.MercadoBitcoin
{
    internal class MercadoBitcoinSchema
    {
        internal class TickerResponse
        {
            public TickerEntryResponse ticker;
        }

        internal class TickerEntryResponse
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal vol;
            public decimal buy;
            public decimal sell;
            public long date;
        }
    }
}
