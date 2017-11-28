using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Abucoins
{
    internal class AbucoinsSchema
    {
        internal class ProductResponse
        {
            public string id;
            public string display_name;
            public string base_currency;
            public string quote_currency;
            public decimal base_min_size;
            public decimal base_max_size;
            public decimal quote_increment;
        }

        internal class TickerResponse
        {
            public int trade_id;
            public decimal price;
            public decimal size;
            public decimal volume;
            public decimal bid;
            public decimal ask;
            public DateTime time;
        }
    }
}
