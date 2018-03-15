using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BitKonan
{
    internal class BitKonanSchema
    {
        #region Private

        internal class BaseResponse
        {
            public string status;
            public string[] errors;
        }

        internal class BalanceResponse
        {
            public BalanceEntryResponse[] data;
        }

        internal class BalanceEntryResponse
        {
            public string currency;
            public BalanceEntryItemResponse balance;
        }

        internal class BalanceEntryItemResponse
        {
            public decimal available;
            public decimal reserved;
            public decimal withdrawal;
            public decimal total;
        }

        internal class NewOrderResponse : BaseResponse
        {
            public NewOrderEntryResponse data;
        }

        internal class NewOrderEntryResponse
        {
            public string order_id;
        }

        internal class OrderInfoResponse
        {
            public OrderInfoEntryResponse[] data;
        }

        internal class OrderInfoEntryResponse
        {
            public string id;
            public decimal amount;
            public decimal price;
            public DateTime time;
            public string type;
            public decimal stop_price;
            public string trade_pair;
        }

        #endregion

        #region Public

        internal class TickerResponse
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal bid;
            public decimal ask;
            public decimal open;
            public decimal volume;
        }

        internal class OrderBookResponse
        {
            public Dictionary<string, decimal>[] bid;
            public Dictionary<string, decimal>[] ask;
        }

        #endregion
    }
}
