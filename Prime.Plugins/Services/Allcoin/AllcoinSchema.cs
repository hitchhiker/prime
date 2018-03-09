using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Allcoin
{
    internal class AllcoinSchema
    {
        #region Private

        internal class ErrorResponse
        {
            public string error_code;
            public bool result;
        }

        internal class UserInfoResponse
        {
            public UserInfoEntryResponse info;
            public bool result;
        }

        internal class UserInfoEntryResponse
        {
            public Dictionary<string, FundItemResponse> funds;
        }

        internal class FundItemResponse
        {
            public decimal btc;
            public decimal usd;
            public decimal ltc;
        }

        internal class OrderInfoResponse
        {
            public bool result;
            public OrderInfoEntryResponse[] orders;
        }

        internal class OrderInfoEntryResponse
        {
            public decimal amount;
            public decimal avg_price;
            public long create_date;
            public decimal deal_amount;
            public string order_id;
            public decimal price;
            public int status;
            public string symbol;
            public string type;
        }

        internal class NewOrderResponse
        {
            public bool result;
            public string order_id;
        }

        #endregion

        #region Public
        internal class TickerResponse
        {
            public long date;
            public TickerEntryResponse ticker;
        }

        internal class TickerEntryResponse
        {
            public decimal buy;
            public decimal high;
            public decimal last;
            public decimal low;
            public decimal sell;
            public decimal vol;
        }

        internal class OrderBookResponse
        {
            public decimal[][] asks;
            public decimal[][] bids;
        }
        #endregion
    }
}
