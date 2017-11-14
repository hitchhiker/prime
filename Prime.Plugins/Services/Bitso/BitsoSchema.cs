using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bitso
{
    internal class BitsoSchema
    {
        internal class TickerResponse
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal vwap;
            public decimal volume;
            public decimal bid;
            public decimal ask;
            public DateTime created_at;
        }

        internal class PayloadResponse
        {
            public string book;
            public decimal minimum_amount;
            public decimal maximum_amount;
            public decimal minimum_price;
            public decimal maximum_price;
            public decimal minimum_value;
            public decimal maximum_value;
        }

        internal class AvailableBooksResponse
        {
            public List<PayloadResponse> payload;
        }
    }
}
