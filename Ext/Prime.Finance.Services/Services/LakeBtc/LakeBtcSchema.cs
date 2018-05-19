using System.Collections.Generic;

namespace Prime.Finance.Services.Services.LakeBtc
{
    internal class LakeBtcSchema
    {
        #region Public
        internal class AllTickersResponse : Dictionary<string, TickerResponse>
        {
        }

        internal class TickerResponse
        {
            public decimal last;
            public decimal? high;
            public decimal? volume;
            public decimal? low;
            public decimal? bid;
            public decimal? ask;
        }

        internal class OrderBookResponse
        {
            public decimal[][] bids;
            public decimal[][] asks;
        }
        #endregion

        #region Private

        internal class ErrorResponse
        {
            public string error;
        }

        internal class UserInfoResponse
        {
            public UserInfoEntryResponse profile;
        }

        internal class UserInfoEntryResponse
        {
            public string email;
            public string uid;
            public string btc_deposit_addres;
        }

        internal class NewOrderResponse
        {
            public string id;
            public string result;
        }

        internal class OrderResponse
        {
            public string id;
            public decimal original_amount;
            public decimal amount;
            public decimal price;
            public string symbol;
            public string type;
            public string state;
            public long at;
        }

        internal class WithdrawalResponse
        {
            public string id;
            public decimal amount;
            public string currency;
            public decimal fee;
            public string state;
            public string source;
            public string external_account_id;
            public long at;
        }

        #endregion
    }
}
