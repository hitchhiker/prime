using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.EthexIndia
{
    internal class EthexIndiaSchema
    {
        internal class TickerResponse
        {
            public string ticker;
            public decimal high;
            public decimal avg;
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
