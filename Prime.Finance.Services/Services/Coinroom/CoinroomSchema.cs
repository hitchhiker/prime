﻿namespace Prime.Finance.Services.Services.Coinroom
{
    internal class CoinroomSchema
    {
        internal class TickerResponse
        {
            public decimal low;
            public decimal high;
            public decimal vwap;
            public decimal volume;
            public decimal bid;
            public decimal ask;
            public decimal last;
        }

        internal class CurrenciesResponse
        {
            public string[] real;
            public string[] crypto;
        }

        internal class OrderBookEntryResponse
        {
            public OrderBookItemResponse[] asks;
            public OrderBookItemResponse[] bids;
        }

        internal class OrderBookItemResponse
        {
            public decimal rate;
            public decimal amount;
            public decimal price;
        }

        internal class OrderBookResponse
        {
            public OrderBookEntryResponse data;
        }
    }
}
