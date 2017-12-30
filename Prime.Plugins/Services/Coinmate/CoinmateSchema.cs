using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Coinmate
{
    internal class CoinmateSchema
    {
        internal class TickerResponse
        {
            public bool error;
            public string errorMessage;
            public TickerEntryResponse data;
        }

        internal class TickerEntryResponse
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal amount;
            public decimal bid;
            public decimal ask;
            public decimal change;
            public decimal open;
            public long timestamp;
        }

        internal class OrderBookEntryResponse
        {
            public OrderBookItemResponse[] asks;
            public OrderBookItemResponse[] bids;
        }

        internal class OrderBookItemResponse
        {
            public decimal price;
            public decimal amount;
        }

        internal class OrderBookResponse
        {
            public long timestamp;
            public bool error;
            public string errorMessage;
            public OrderBookEntryResponse data;
        }
    }
}
