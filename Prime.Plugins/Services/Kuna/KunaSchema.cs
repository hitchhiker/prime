using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Kuna
{
    internal class KunaSchema
    {
        internal class AllTickersResponse : Dictionary<string, TickerResponse>
        {

        }

        internal class TickerResponse
        {
            public long at;
            public TickerEntryResponse ticker;
        }

        internal class TickerEntryResponse
        {
            public decimal buy;
            public decimal sell;
            public decimal low;
            public decimal high;
            public decimal last;
            public decimal vol;
            public decimal price;
        }
    }
}
