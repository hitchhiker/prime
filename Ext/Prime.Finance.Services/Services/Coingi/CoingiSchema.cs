using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Prime.Finance.Services.Services.Coingi
{
    internal class CoingiSchema
    {
        #region Public
        internal class OrderBookResponse
        {
            public OrderBookItemResponse[] asks;
            public OrderBookItemResponse[] bids;
        }

        internal class OrderBookItemResponse
        {
            public CurrencyPair currencyPair;
            public decimal price;
            public decimal baseAmount;
            public decimal counterAmount;
        }

        internal class CurrencyPair
        {
            [JsonProperty("base")]
            public string baseAsset;
            public string counter;
        }

        internal class TransactionResponse
        {
            public string id;
            public long timestamp;
            public decimal amount;
            public decimal price;
            public CurrencyPair currencyPair;
        }
        #endregion

        #region Private

        internal class ErrorResponse
        {
            public ErrorInfoResponse[] errors;
        }

        internal class ErrorInfoResponse
        {
            public int code;
            public string message;
        }

        internal class CurrencyResponse
        {
            public string name;
            public string crypto;
        }

        internal class BalanceResponse
        {
            public CurrencyResponse currency;
            public decimal available;
            public decimal inOrders;
            public decimal deposited;
            public decimal withdrawing;
            public decimal blocked;
        }

        internal class NewOrderResponse
        {
            public string result;
        }

        internal class ActiveOrdersResponse
        {
            public OrderResponse[] orders;
        }

        internal class OrderResponse
        {
            public string id;
            public int type;
            public long timestamp;
            public decimal price;
            public decimal baseAmount;
            public decimal counterAmount;
            public decimal originalBaseAmount;
            public decimal originalCounterAmount;
            public int status;
            public CurrencyPair currencyPair;
        }

        internal class WithdrawalResponse
        {
            public bool result;
        }
        #endregion
    }
}
