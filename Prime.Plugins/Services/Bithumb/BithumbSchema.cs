using System.Collections.Generic;

namespace Prime.Plugins.Services.Bithumb
{
    internal class BithumbSchema
    {
        #region Base

        internal class BaseResponse<TData>
        {
            public short status;
            public TData data;
            public string message;
        }

        #endregion

        #region Private


        #endregion

        #region Public

        internal class TickersResponse : BaseResponse<Dictionary<string, object>> { }

        internal class SingleTickerResponse : BaseResponse<TickerResponse> { }

        internal class TickerResponse
        {
            public decimal opening_price;
            public decimal closing_price;
            public decimal min_price;
            public decimal max_price;
            public decimal average_price;
            public decimal units_traded;
            public decimal volume_1day;
            public decimal volume_7day;
            public decimal buy_price;
            public decimal sell_price;
            public long? date;
        }

        internal class OrderBookResponse : BaseResponse<OrderBookDataResponse> { }

        internal class OrderBookDataResponse
        {
            public long timestamp;
            public string order_currency;
            public string payment_currency;
            public OrderBookRecordResponse[] bids;
            public OrderBookRecordResponse[] asks;
        }

        internal class OrderBookRecordResponse
        {
            public decimal quantity;
            public decimal price;
        }

        #endregion
    }
}