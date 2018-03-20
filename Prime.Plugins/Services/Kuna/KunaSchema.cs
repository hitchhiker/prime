using System;
using System.Collections.Generic;
using System.Text;
using RestEase;

namespace Prime.Plugins.Services.Kuna
{
    internal class KunaSchema
    {
        #region Private
        internal class ErrorResponse
        {
            public ErrorEntryResponse error;
        }

        internal class ErrorEntryResponse
        {
            public int code;
            public string message;
        }

        internal class UserInfoResponse
        {
            public string email;
            public bool activated;
        }

        internal class OrderInfoResponse
        {
            public string id;
            public string side;
            public string ord_type;
            public decimal price;
            public decimal avg_price;
            public string state;
            public string market;
            public long created_at;
            public decimal volume;
            public decimal remaining_volume;
            public decimal executed_volume;
            public int trades_count;
        }

        internal class ActiveOrdersResponse : List<OrderInfoResponse>
        {
            
        }
        #endregion

        #region Public
        internal class AllTickersResponse : Dictionary<string, TickerResponse>
        {

        }

        internal class TickerResponse
        {
            public long at;
            public TickerEntryResponse ticker;
        }

        internal class TickerEntryResponse
        {
            public decimal buy;
            public decimal sell;
            public decimal low;
            public decimal high;
            public decimal last;
            public decimal vol;
            public decimal price;
        }

        internal class OrderBookItemResponse
        {
            public int id;
            public int trades_count;
            public decimal price;
            public decimal avg_price;
            public decimal volume;
            public decimal remaining_volume;
            public decimal executed_volume;
            public string side;
            public string ord_type;
            public string state;
            public string market;
            public DateTime created_at;
        }

        internal class OrderBookResponse
        {
            public OrderBookItemResponse[] asks;
            public OrderBookItemResponse[] bids;
        }
        #endregion
    }
}
