using System;
using System.Collections.Generic;

namespace Prime.Plugins.Services.Gdax
{
    internal class GdaxSchema
    {
        internal class ProductsResponse : List<ProductResponse> { }

        internal class ProductTickerResponse
        {
            public long trade_id;
            public decimal price;
            public decimal size;
            public decimal bid;
            public decimal ask;
            public decimal volume;
            public DateTime time;
        }

        internal class ProductResponse
        {
            public string id;
            public string base_currency;
            public string quote_currency;
            public decimal base_min_size;
            public decimal base_max_size;
            public decimal quote_increment;
        }

        internal class OrderBookResponse
        {
            public string sequence;
            public string[][] bids;
            public string[][] asks;
        }

        internal class InvalidResponse
        {
            public string message;
        }
    }
}
