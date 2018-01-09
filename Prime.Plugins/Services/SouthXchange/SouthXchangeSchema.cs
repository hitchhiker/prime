using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.SouthXchange
{
    internal class SouthXchangeSchema
    {
        internal class AllTickerResponse
        {
            public string Market;
            public decimal? Bid;
            public decimal? Ask;
            public decimal? Last;
            public decimal? Variation24Hr;
            public decimal? Volume24Hr;
        }

        internal class TickerResponse
        {
            public decimal? Bid;
            public decimal? Ask;
            public decimal? Last;
            public decimal? Variation24Hr;
            public decimal? Volume24Hr;
        }
        
        internal class OrderBookItemResponse
        {
            public int Index;
            public decimal Amount;
            public decimal Price;
        }

        internal class OrderBookResponse
        {
            public OrderBookItemResponse[] BuyOrders;
            public OrderBookItemResponse[] SellOrders;
        }
    }
}
