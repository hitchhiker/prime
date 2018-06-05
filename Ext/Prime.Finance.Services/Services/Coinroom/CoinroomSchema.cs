using System.Collections.Generic;

namespace Prime.Finance.Services.Services.Coinroom
{
    internal class CoinroomSchema
    {
        #region Public
        internal class TickerResponse
        {
            public decimal low;
            public decimal high;
            public decimal vwap;
            public decimal volume;
            public decimal bid;
            public decimal ask;
            public decimal last;
        }

        internal class CurrenciesResponse
        {
            public string[] real;
            public string[] crypto;
        }

        internal class OrderBookEntryResponse
        {
            public OrderBookItemResponse[] asks;
            public OrderBookItemResponse[] bids;
        }

        internal class OrderBookItemResponse
        {
            public decimal rate;
            public decimal amount;
            public decimal price;
        }

        internal class OrderBookResponse
        {
            public OrderBookEntryResponse data;
        }
        #endregion

        #region Private
        
        internal class BaseResponse<T>
        {
            public bool result;
            public T data;
            public string[] errors;
        }

        internal class ErrorResponse
        {
            public bool result;
            public object data;
            public string[] errors;
        }

        internal class BalancesResponse : BaseResponse<BalanceEntryResponse>
        {

        }

        internal class BalanceEntryResponse
        {
            public Dictionary<string, decimal> balance;
        }

        internal class OrdersResponse : BaseResponse<OrderEntryResponse[]>
        {

        }

        //TODO - SC - Documentation does not specify response example, so this is just an assumption
        internal class OrderEntryResponse
        {
            public string id;
            public decimal amount;
            public decimal rate;
            public string type;
            public string status;
        }
        #endregion
    }
}
