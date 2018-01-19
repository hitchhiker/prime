using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.Binance
{
    internal class BinanceSchema
    {
        #region Base

        internal class ErrorResponseBase
        {
            public int code;
            public string msg;
        }

        #endregion

        #region Private

        internal class UserInformationResponse : ErrorResponseBase
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

        internal class UserBalanceResponse : ErrorResponseBase
        {
            public string asset;
            public decimal free;
            public decimal locked;
        }

        internal class NewOrderResponse : ErrorResponseBase
        {
            public string symbol;
            public long orderId;
            public string clientOrderId;
            public long transactTime;
        }

        #endregion

        #region Public

        internal class LatestPricesResponse : List<LatestPriceResponse>
        {
            // Copy of ErrorResponseBase.
            public int code;
            public string msg;
        }

        internal class CandlestickResponse : List<decimal[]>
        {
            // Copy of ErrorResponseBase.
            public int code;
            public string msg;
        }

        internal class Ticker24HrResponse : ErrorResponseBase
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

        internal class OrderBookResponse : ErrorResponseBase
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

        #endregion
    }
}
