using System.Collections.Generic;

namespace Prime.Finance.Services.Services.Braziliex
{
    internal class BraziliexSchema
    {
        #region Base

        internal class BaseResponse
        {
            public int success;
            public string message;
        }
        #endregion

        #region Private

        internal class BalancesResponse
        {
            public Dictionary<string, decimal> balance;
        }

        internal class NewOrderResponse : BaseResponse
        {
            public string order_number;
        }

        internal class ActiveOrdersResponse
        {
            public ActiveOrderEntryResponse[] order_open;
        }

        internal class ActiveOrderEntryResponse
        {
            public string order_number;
            public string type;
            public string market;
            public decimal price;
            public decimal amount;
            public decimal total;
            public decimal progress;
            public long date;
        }
        #endregion

        #region Public
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
        #endregion
    }
}
