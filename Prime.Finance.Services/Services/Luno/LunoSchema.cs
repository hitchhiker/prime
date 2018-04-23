﻿namespace Prime.Finance.Services.Services.Luno
{
    internal class LunoSchema
    {
        internal class AllTickersResponse
        {
            public TickerResponse[] tickers;
        }

        internal class TickerResponse
        {
            public string pair;
            public decimal bid;
            public decimal ask;
            public decimal last_trade;
            public decimal rolling_24_hour_volume;
            public long timestamp;
        }
        
        internal class OrderBookItemResponse
        {
            public decimal volume;
            public decimal price;
        }

        internal class OrderBookResponse
        {
            public long timestamp;
            public OrderBookItemResponse[] bids;
            public OrderBookItemResponse[] asks;
        }
    }
}
