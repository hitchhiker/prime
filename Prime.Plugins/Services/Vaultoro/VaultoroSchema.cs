using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Prime.Plugins.Services.Vaultoro
{
    internal class VaultoroSchema
    {
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

        internal class OrderBookEntryResponse
        {
            public OrderBookItemResponse[] b;
            public OrderBookItemResponse[] s;
        }

        internal class OrderBookItemResponse
        {
            public decimal Gold_Price;
            public decimal Gold_Amount;
        }

        internal class OrderBookResponse
        {
            public string status;
            public OrderBookEntryResponse data;
        }
    }
}
