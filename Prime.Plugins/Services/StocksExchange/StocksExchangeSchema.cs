using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.StocksExchange
{
    internal class StocksExchangeSchema
    {
        internal class AllTickersResponse
        {
            public decimal min_order_amount;
            public decimal ask;
            public decimal bid;
            public decimal last;
            public decimal lastDayAgo;
            public decimal vol;
            public decimal spread;
            public decimal buy_fee_percent;
            public decimal sell_fee_percent;
            public string market_name;
            public long updated_time;
            public long Server_time;
        }
    }
}
