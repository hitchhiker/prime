﻿using System.Collections.Generic;

namespace Prime.Finance.Services.Services.Braziliex
{
    internal class BraziliexSchema
    {
        internal class AllTickersResponse : Dictionary<string, TickerResponse>
        {
        }

        internal class TickerResponse
        {
            public int active;
            public string market;
            public decimal last;
            public decimal percentChange;
            public decimal baseVolume;
            public decimal quoteVolume;
            public decimal highestBid;
            public decimal lowestAsk;
            public decimal baseVolume24;
            public decimal quoteVolume24;
            public decimal highestBid24;
            public decimal lowestAsk24;
        }

        internal class OrderBookResponse
        {
            public OrderBookItemResponse[] bids;
            public OrderBookItemResponse[] asks;
        }

        internal class OrderBookItemResponse
        {
            public decimal price;
            public decimal amount;
        }
    }
}
