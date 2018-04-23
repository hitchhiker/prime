﻿namespace Prime.Finance.Services.Services.LiveCoin
{
    internal class LiveCoinSchema
    {
        internal class TickerResponse
        {
            public string curr;
            public string symbol;
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal volume;
            public decimal vwap;
            public decimal max_bid;
            public decimal min_ask;
            public decimal best_bid;
            public decimal best_ask;
        }

        internal class OrderBookResponse
        {
            public long timestamp;
            public float[][] bids;
            public float[][] asks;
        }
    }
}
