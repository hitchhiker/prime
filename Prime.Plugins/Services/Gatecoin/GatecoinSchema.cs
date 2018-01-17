using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Gatecoin
{
    internal class GatecoinSchema
    {
        internal class TickerResponse
        {
            public TickerEntryResponse ticker;
            public StatusResponse responseStatus;
        }

        internal class TickersResponse
        {
            public TickerEntryResponse[] tickers;
            public StatusResponse responseStatus;
        }

        internal class TickerEntryResponse
        {
            public string currencyPair;
            public decimal open;
            public decimal last;
            public decimal lastQ;
            public decimal high;
            public decimal low;
            public decimal volume;
            public decimal bid;
            public decimal bidQ;
            public decimal ask;
            public decimal askQ;
            public decimal vwap;
            public long createDateTime;
        }

        internal class StatusResponse
        {
            public string message;
        }
        
        internal class OrderBookItemResponse
        {
            public decimal price;
            public decimal volume;
        }

        internal class OrderBookResponse
        {
            public StatusResponse responseStatus;
            public OrderBookItemResponse[] asks;
            public OrderBookItemResponse[] bids;
        }
    }
}
