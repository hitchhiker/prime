using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BxInTh
{
    internal class BxInThSchema
    {
        internal class TickerResponse : CurrencyResponse
        {
            //public int pairing_id;
            //public string primary_currency;
            //public string secondary_currency;
            public decimal change;
            public decimal last_price;
            public decimal volume_24hours;
            public OrderBook orderbook;
        }

        internal class OrderBook
        {
            public OrderBookEntry bids;
            public OrderBookEntry asks;
        }

        internal class OrderBookEntry
        {
            public decimal total;
            public decimal volume;
            public decimal highbid;
        }

        internal class CurrencyResponse
        {
            public int pairing_id;
            public string primary_currency;
            public string secondary_currency;
        }
    }
}
