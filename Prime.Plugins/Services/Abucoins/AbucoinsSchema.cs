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

        internal class VolumeResponse
        {
            public string product_id;
            public decimal last;
            public decimal open;
            public decimal high;
            public decimal low;
            public decimal volume;
            public decimal volume_BTC;
            public decimal volume_USD;
            public decimal volume_7d;
            public decimal volume_30d;
            public decimal change;
        }

        internal class OrderBookResponse
        {
            public int? sequence;
            public decimal[][] bids;
            public decimal[][] asks;
        }
    }
}
