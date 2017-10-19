using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.Binance
{
    internal class BinanceSchema
    {
        internal class LatestPricesResponse : List<LatestPriceResponse> { }

        internal class OrderBookResponse
        {
            public long lastUpdateId;
            public object[][] bids;
            public object[][] asks;
        }

        internal class LatestPriceResponse
        {
            public string symbol;
            public decimal price;
        }
    }
}
