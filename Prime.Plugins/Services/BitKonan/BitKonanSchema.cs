using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BitKonan
{
    internal class BitKonanSchema
    {
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

        internal class BalanceResponse
        {
            public string status;
            public BalanceEntryResponse[] data;
            public string[] errors;
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

        internal class NewOrderResponse
        {
            public string status;
            public NewOrderEntryResponse data;
            public string[] errors;
        }

        internal class NewOrderEntryResponse
        {
            public string order_id;
        }

        internal class OrderInfoResponse
        {
            public string status;
            public OrderInfoEntryResponse[] data;
            public string[] errors;
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
    }
}
