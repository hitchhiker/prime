using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.HitBtc
{
    internal class HitBtcSchema
    {
        #region Base

        internal class ErrorResponse
        {
            public int code;
            public string message;
            public string description;
        }

        internal class BaseResponse
        {
            public ErrorResponse error;
        }

        #endregion

        #region Private

        internal class BalancesResponse : List<BalanceResponse>
        {
            public ErrorResponse error;
        }

        internal class BalanceResponse
        {
            public string currency;
            public decimal available;
            public decimal reserved;
        }

        internal class DepositAddressResponse : BaseResponse
        {
            public string address;
            public string paymentId;
        }

        internal class ActiveOrderInfoResponse : BaseResponse
        {
            public int id;
            public string clientOrderId;
            public string symbol;
            public string side;
            public string status;
            public string type;
            public string timeInForce;
            public decimal quantity;
            public decimal price;
            public decimal cumQuantity;
            public DateTime createdAt;
            public DateTime updatedAt;
        }

        internal class WithdrawCryptoResponse : BaseResponse
        {
            public string id;
        }

        #endregion

        #region Public

        internal class SymbolsResponse : List<SymbolResponse>
        {
            public ErrorResponse error;
        }

        internal class SymbolResponse
        {
            public string id;
            public string baseCurrency;
            public string quoteCurrency;
            public decimal quantityIncrement;
            public decimal tickSize;
            public decimal takeLiquidityRate;
            public decimal provideLiquidityRate;
            public string feeCurrency;
        }

        internal class TickersResponse : List<TickerResponse>
        {
            public ErrorResponse error;
        }

        internal class TickerResponse : BaseResponse
        {
            public decimal? ask;
            public decimal? bid;
            public decimal? last;
            public decimal? open;
            public decimal? low;
            public decimal? high;
            public decimal? volume;
            public decimal? volumeQuote;
            public DateTime timestamp;
            public string symbol;
        }

        #endregion
    }
}
