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
        #endregion
    }
}
