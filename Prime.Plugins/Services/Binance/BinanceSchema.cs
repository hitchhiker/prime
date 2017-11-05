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

        internal class Ticker24HrResponse
        {
            public decimal priceChange;
            public decimal priceChangePercent;
            public decimal weightedAvgPrice;
            public decimal prevClosePrice;
            public decimal lastPrice;
            public decimal bidPrice;
            public decimal askPrice;
            public decimal openPrice;
            public decimal highPrice;
            public decimal lowPrice;
            public decimal volume;
            public long openTime;
            public long closeTime;
            public int fristId;
            public int lastId;
            public int count;
        }

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
