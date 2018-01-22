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
            public bool? success;
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

        internal class QueryOrderResponse : ErrorResponseBase
        {
            public string symbol;
            public int orderId;
            public string clientOrderId;
            public decimal price;
            public decimal origQty;
            public decimal executedQty;
            public string status;
            public string timeInForce;
            public string type;
            public string side;
            public decimal stopPrice;
            public decimal icebergQty;
            public long time;
        }

        internal class DepositHistoryResponse : ErrorResponseBase
        {
            public DepositListEntryResponse[] depositList;
        }

        internal class DepositListEntryResponse
        {
            public long insertTime;
            public decimal amount;
            public string asset;
            public string address;
            public string txId;
            public int status;
        }

        internal class WithdrawalRequestResponse : ErrorResponseBase
        {
            public string id;
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
