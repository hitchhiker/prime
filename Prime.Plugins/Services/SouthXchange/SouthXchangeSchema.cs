using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.SouthXchange
{
    internal class SouthXchangeSchema
    {
        #region Public
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
        #endregion

        #region Private
        internal class OrderInfoResponse
        {
            public string Code;
            public string Type;
            public decimal Amount;
            public decimal OriginalAmount;
            public decimal LimitPrice;
            public string ListingCurrency;
            public string ReferenceCurrency;
        }

        internal class BalanceResponse
        {
            public string Currency;
            public decimal Deposited;
            public decimal Available;
            public decimal Unconfirmed;
        }
        #endregion
    }
}
