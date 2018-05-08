using System.Collections.Generic;
using Newtonsoft.Json;

namespace Prime.Finance.Services.Services.Vaultoro
{
    internal class VaultoroSchema
    {
        #region Base
        internal class BaseResponse<T>
        {
            public string status;
            public T data;
        }

        internal class ErrorResponse : BaseResponse<ErrorEntryResponse>
        {
        }

        internal class ErrorEntryResponse
        {
            public string message;
        }
        #endregion

        #region Public
        internal class MarketResponse
        {
            public string status;
            public MarketEntryResponse data;
        }

        internal class MarketEntryResponse
        {
            public string MarketCurrency;
            public string BaseCurrency;
            public string MarketCurrencyLong;
            public string BaseCurrencyLong;
            public string MarketName;
            public bool IsActive;
            public float MinTradeSize;
            public decimal MinUnitQty;
            public decimal MinPrice;
            public decimal LastPrice;

            [JsonProperty("24hLow")]
            public decimal Low24h;

            [JsonProperty("24hHigh")]
            public decimal High24h;

            [JsonProperty("24HVolume")]
            public decimal Volume24h;
        }

        internal class OrderBookItemResponse
        {
            public decimal Gold_Price;
            public decimal Gold_Amount;
        }

        internal class OrderBookEntryResponse
        {
            public OrderBookItemResponse[] b;
            public OrderBookItemResponse[] s;
        }

        internal class OrderBookResponse
        {
            public string status;
            public OrderBookEntryResponse[] data;
        }
        #endregion

        #region Private

        internal class BalanceResponse : BaseResponse<BalanceEntryResponse[]>
        {

        }

        internal class BalanceEntryResponse
        {
            public string currency_code;
            public decimal cash;
            public decimal reserved;
        }

        internal class OrderResponse
        {
            public string action;
            public string Order_ID;
            public string type;
            public string time;
            public decimal price;
            public decimal btc;
            public decimal gld;
        }

        internal class OrdersResponse : BaseResponse<OrdersEntryResponse[]>
        {

        }

        internal class OrdersEntryResponse 
        {
            public OrderInfoResponse[] b;
            public OrderInfoResponse[] s;
        }

        internal class OrderInfoResponse
        {
            public string Order_ID;
            public decimal BTC_Amount;
            public decimal Gold_Price;
            public decimal Gold_Amount;
        }

        internal class WithdrawalResponse : BaseResponse<WithdrawalEntryResponse>
        {
        }

        internal class WithdrawalEntryResponse
        {
            public decimal amount;
            public string address;
        }
        #endregion
    }
}
