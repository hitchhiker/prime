using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Kucoin
{
    internal class KucoinSchema
    {
        #region Base

        internal class BaseResponse<TResult>
        {
            public string msg;
            public string code;
            public bool success;
            public TResult data;
        }

        #endregion

        #region Public

        internal class TickerResponse
        {
            public string coinType;
            public bool trading;
            public decimal lastDealPrice;
            public decimal buy;
            public decimal sell;
            public string coinTypePair;
            public string symbol;
            public decimal sort;
            public decimal feeRate;
            public decimal volValue;
            public decimal high;
            public decimal datetime;
            public decimal vol;
            public decimal low;
            public decimal changeRate;
        }

        internal class OrderBookResponse
        {
            public decimal[][] SELL;
            public decimal[][] BUY;
        }

        #endregion

        #region Private

        internal class UserInfoResponse
        {
            public UserInfoEntryResponse data;

        }

        internal class UserInfoEntryResponse
        {
            public string referrer_code;
            public bool photoCredentialValidated;
            public bool videoValidated;
            public string language;
            public string currency;
            public string oid;
            public decimal baseFeeRate;
            public bool hasCredential;
            public string credentialNumber;
            public bool phoneValidated;
            public string phone;
            public bool credentialValidated;
            public bool googleTwoFaBinding;
            public string nickname;
            public string name;
            public bool hasTradePassword;
            public bool emailValidated;
            public string email;
        }

        #endregion
    }
}
