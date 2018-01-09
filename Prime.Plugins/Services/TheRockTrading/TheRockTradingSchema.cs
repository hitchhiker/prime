using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.TheRockTrading
{
    internal class TheRockTradingSchema
    {
        internal class AllTickersResponse
        {
            public TickerResponse[] tickers;
        }

        internal class TickerResponse
        {
            public string fund_id;
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal open;
            public decimal close;
            public decimal volume;
            public decimal volume_traded;
            public decimal bid;
            public decimal ask;
            public DateTime date;
        }
        
        internal class OrderBookItemResponse
        {
            public decimal price;
            public decimal amount;
        }

        internal class OrderBookResponse
        {
            public string fund_id;
            public DateTime date;
            public OrderBookItemResponse[] asks;
            public OrderBookItemResponse[] bids;
        }
    }
}
