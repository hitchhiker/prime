using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.CoinCorner
{
    internal class CoinCornerSchema
    {
        internal class TickerResponse
        {
            public decimal LastPrice;
            public decimal AvgPrice;
            public decimal LastHigh;
            public decimal LastLow;
            public decimal Volume;
            public decimal BidHigh;
            public decimal BidLow;
            public string Coin;
            public DateTime CreateDate;
        }

        internal class OrderBookResponse
        {
            public string Coin;
            public OrderBookItemResponse[] buys;
            public OrderBookItemResponse[] sells;
        }

        internal class OrderBookItemResponse
        {
            public decimal Amount;
            public decimal Price;
        }
    }
}
