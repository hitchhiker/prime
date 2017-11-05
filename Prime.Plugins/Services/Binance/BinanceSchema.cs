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

        internal class CandlestickResponse : List<decimal[]> { }

        internal class UserInformationResponse
        {
            public decimal makerCommission;
            public decimal takerCommission;
            public decimal buyerCommission;
            public decimal sellerCommission;
            public bool canTrade;
            public bool canWithdraw;
            public bool canDeposit;
            public UserBalanceResponse[] balances;
        }

        internal class UserBalanceResponse
        {
            public string asset;
            public decimal free;
            public decimal locked;
        }

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

            public override string ToString()
            {
                return $"{symbol}: {price}";
            }
        }
    }
}
