using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bitso
{
    internal class BitsoSchema
    {
        internal class BaseResponse<T>
        {
            public bool success;
            public T payload;
        }

        internal class AllTickerResponse : BaseResponse<TickerEntryResponse[]>
        {
            
        }

        internal class TickerResponse : BaseResponse<TickerEntryResponse>
        {

        }

        internal class TickerEntryResponse
        {
            public string book;
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal vwap;
            public decimal volume;
            public decimal bid;
            public decimal ask;
            public DateTime created_at;
        }

        internal class OrderBookEntryResponse
        {
            public long sequence;
            public DateTime updated_at;
            public OrderBookItemResponse[] bids;
            public OrderBookItemResponse[] asks;
        }

        internal class OrderBookItemResponse
        {
            public decimal price;
            public decimal amount;
            public string book;
        }

        internal class OrderBookResponse
        {
            public bool success;
            public OrderBookEntryResponse payload;
        }
    }
}
