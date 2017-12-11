using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Bitfinex
{
    internal class BitfinexSchema
    {
        #region Base

        internal class FeesBaseResposnse
        {
            public decimal maker_fees;
            public decimal taker_fees;
        }

        internal class BaseRequest
        {
            public string request;
            public string nonce;
        }

        #endregion


        #region Private

        internal class AccountInfoResponse : List<AccountInfoDataResponse> { }

        internal class AccountInfoDataResponse : FeesBaseResposnse
        {
            public FeesResponse fees;
        }

        internal class FeesResponse : List<FeeResponse> { }

        internal class FeeResponse : FeesBaseResposnse
        {
            public string pairs;
        }

        internal class WalletBalanceResponse : List<WalletBalancesResponse> { }

        internal class WalletBalancesResponse
        {
            public string type;
            public string currency;
            public decimal amount;
            public decimal available;
        }

        internal class AccountInfoRequest : BaseRequest { }
        internal class WalletBalancesRequest : BaseRequest { }

        #endregion

        #region Public

        internal class TickerResponse
        {
            public decimal mid;
            public decimal bid;
            public decimal ask;
            public decimal last_price;
            public decimal low;
            public decimal high;
            public decimal volume;
            public decimal timestamp;
        }

        #endregion
    }
}
