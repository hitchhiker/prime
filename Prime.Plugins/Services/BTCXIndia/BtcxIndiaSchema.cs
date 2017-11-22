using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BTCXIndia
{
    internal class BtcxIndiaSchema
    {
        internal class TickerResponse
        {
            public decimal avg;
            public decimal high;
            public decimal low;
            public decimal total_volume_24h;
            public decimal current_volume;
            public decimal last_traded_price;
            public decimal bid;
            public decimal ask;
            public long last_traded_time;
        }
    }
}
