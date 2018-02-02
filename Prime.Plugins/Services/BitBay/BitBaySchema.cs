using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BitBay
{
    internal class BitBaySchema
    {
        #region Public
        internal class TickerResponse
        {
            public decimal max;
            public decimal min;
            public decimal last;
            public decimal bid;
            public decimal ask;
            public decimal vwap;
            public decimal average;
            public decimal volume;
        }

        internal class OrderBookResponse
        {
            public decimal[][] bids;
            public decimal[][] asks;
        }
        #endregion

        #region Private
        internal class UserInfoResponse
        {
            public bool success;
            public decimal fee;
            public Dictionary<string, BalanceEntryResponse> balances;
        }

        internal class BalanceEntryResponse
        {
            public int available;
            public int locked;
        }

        internal class ErrorBaseResponse
        {
            public string code;
            public string message;
        }

        internal class NewOrderResponse : BitBaySchema.ErrorBaseResponse
        {
            public string order_id;
            public string error;
        }

        internal class WithdrawalRequestResponse : BitBaySchema.ErrorBaseResponse
        {
            public bool success;
            public string error;
        }

        internal class OrdersResponse : BitBaySchema.ErrorBaseResponse
        {
            public string order_id;
            public string order_currency;
            public long order_date;
            public string payment_currency;
            public string type;
            public string status;
            public decimal units;
            public decimal start_units;
            public decimal current_price;
            public decimal start_price;
        }
        #endregion
    }
}
