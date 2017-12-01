using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BxInTh
{
    internal class BxInThSchema
    {
        internal class TickersResponse : Dictionary<int, BxInThSchema.TickerResponse> { }

        internal class TickerResponse : CurrencyResponse
        {
            public decimal change;
            public decimal last_price;
            public decimal volume_24hours;
            public OrderBookResponse orderbook;
        }

        internal class OrderBookResponse
        {
            public OrderBookEntryResponse bids;
            public OrderBookEntryResponse asks;
        }

        internal class OrderBookEntryResponse
        {
            public decimal total;
            public decimal volume;
            public decimal highbid;
        }

        internal class CurrenciesResponse : Dictionary<int, CurrencyResponse> { }

        internal class CurrencyResponse
        {
            public int pairing_id;
            public string primary_currency;
            public string secondary_currency;
        }
    }
}
