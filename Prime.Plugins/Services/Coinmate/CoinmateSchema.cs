using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Coinmate
{
    internal class CoinmateSchema
    {
        #region Base
        
        internal class BaseResponse<T>
        {
            public bool error;
            public string errorMessage;
            public T data;
        }
        
        #endregion

        #region Private
        
        internal class BalanceResponse : BaseResponse<Dictionary<string, BalanceEntryResponse>>
        {
        }

        internal class BalanceEntryResponse
        {
            public string currency;
            public decimal balance;
            public decimal reserved;
            public decimal available;
        }

        internal class NewOrderResponse : BaseResponse<string>
        {
        }

        internal class OrderInfoResponse : BaseResponse<OrderInfoEntryResponse[]>
        {
        }

        internal class OrderInfoEntryResponse
        {
            public string id;
            public long timestamp;
            public string type;
            public string currencyPair;
            public decimal price;
            public decimal amount;
            public decimal remainingAmount;
            public decimal originalAmount;
            public string status;
        }

        internal class WithdrawalRequestResponse : BaseResponse<string>
        {
        }

        #endregion

        #region Public
        
        internal class TickerResponse
        {
            public bool error;
            public string errorMessage;
            public TickerEntryResponse data;
        }

        internal class TickerEntryResponse
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal amount;
            public decimal bid;
            public decimal ask;
            public decimal change;
            public decimal open;
            public long timestamp;
        }

        internal class OrderBookEntryResponse
        {
            public OrderBookItemResponse[] asks;
            public OrderBookItemResponse[] bids;
        }

        internal class OrderBookItemResponse
        {
            public decimal price;
            public decimal amount;
        }

        internal class OrderBookResponse
        {
            public long timestamp;
            public bool error;
            public string errorMessage;
            public OrderBookEntryResponse data;
        }
        
        #endregion
    }
}
