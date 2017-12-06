using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Exmo
{
    internal class ExmoSchema
    {
        internal class TickerResponse : Dictionary<string, TickerEntryResponse> { }

        internal class TickerEntryResponse
        {
            public decimal avg;
            public decimal high;
            public decimal low;
            public decimal last_trade;
            public decimal vol;
            public decimal vol_curr;
            public decimal buy_price;
            public decimal sell_price;
            public long updated;
        }
    }
}
