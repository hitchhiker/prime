using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Kucoin
{
    internal class KucoinSchema
    {
        #region Base

        internal class ErrorBaseResponse : BaseResponse
        {
            public long timestamp;
            public short status;
            public string error;
            public string message;
            public string path;
        }

        internal class BaseResponse
        {
            public string msg;
            public string code;
            public bool success;
        }

        #endregion

        #region Public

        internal class TickersResponse : ErrorBaseResponse
        {
            public TickerDataResponse[] data;
        }

        internal class TickerResponse : ErrorBaseResponse
        {
            public TickerDataResponse data;
        }

        internal class TickerDataResponse
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

        internal class OrderBookResponse : ErrorBaseResponse
        {
            public OrderBookDataResponse data;
        }
    
        internal class OrderBookDataResponse
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
            public LoginRecordResponse loginRecord;
        }

        internal class LoginRecordResponse
        {
            public LoginRecordEntryResponse last;
            public LoginRecordEntryResponse current;
        }

        internal class LoginRecordEntryResponse
        {
            public string ip;
            public string context;
            public long time;
        }

        internal class NewOrderResponse : KucoinSchema.ErrorBaseResponse
        {
            public NewOrderDataResponse data;
        }

        internal class NewOrderDataResponse
        {
            public string orderOid;
        }

        internal class ActiveOrdersResponse : KucoinSchema.ErrorBaseResponse
        {
            public ActiveOrdersDataResponse data;
        }

        internal class ActiveOrdersDataResponse
        {
            public ActiveOrderEntryResponse[] SELL;
            public ActiveOrderEntryResponse[] BUY;
        }

        internal class ActiveOrderEntryResponse
        {
            public string oid;
            public string type;
            public string userOid;
            public string coinType;
            public string coinTypePair;
            public string direction;
            public decimal price;
            public decimal dealAmount;
            public decimal pendingAmount;
            public long createdAt;
            public long updatedAt;
        }

        internal class DealtOrdersResponse : KucoinSchema.ErrorBaseResponse
        {
            public DealtOrdersDataResponse data;
        }

        internal class DealtOrdersDataResponse
        {
            public DealtOrderEntryResponse[] datas;
        }

        internal class WithdrawalRequestResponse : KucoinSchema.ErrorBaseResponse
        {
            
        }

        internal class DealtOrderEntryResponse
        {
            public string oid;
            public decimal dealPrice;
            public string orderOid;
            public string direction;
            public decimal amount;
            public decimal dealValue;
            public long createdAt;
        }

        #endregion
    }
}
