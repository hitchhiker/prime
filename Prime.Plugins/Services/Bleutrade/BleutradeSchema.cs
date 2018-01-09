using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bleutrade
{
    internal class BleutradeSchema
    {
        internal class BaseResponse<TResult>
        {
            public bool success;
            public string message;
            public TResult result;
        }

        internal class MarketEntryResponse
        {
            public bool IsActive;
            public string MarketCurrency;
            public string BaseCurrency;
            public string MarketName;
            public decimal PrevDay;
            public decimal High;
            public decimal Low;
            public decimal Last;
            public decimal Average;
            public decimal Volume;
            public decimal BaseVolume;
            public decimal Bid;
            public decimal Ask;
            public DateTime TimeStamp;
        }

        internal class TickerEntryResponse
        {
            public decimal Last;
            public decimal Bid;
            public decimal Ask;
        }

        internal class OrderBookEntryResponse
        {
            public OrderBookItemResponse[] buy;
            public OrderBookItemResponse[] sell;
        }

        internal class OrderBookItemResponse
        {
            public decimal rate;
            public decimal quantity;
        }

        internal class OrderBookResponse
        {
            public bool success;
            public string message;
            public OrderBookEntryResponse result;
        }
    }
}
