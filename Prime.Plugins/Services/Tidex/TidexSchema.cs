using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Prime.Plugins.Services.Tidex
{
    internal class TidexSchema
    {
        #region Base

        internal class BaseResponse<T>
        {
            public byte success;

            [JsonProperty("return")]
            public T return_;

            public string error;
            public StatResponse stat;
        }

        internal class StatResponse
        {
            public bool isSuccess;
            public string serverTime;
            public string time;
            public string errors;
        }

        #endregion

        #region Private

        internal class UserInfoExtResponse : BaseResponse<UserInfoDataExtResponse> { }

        internal class UserInfoDataExtResponse
        {
            //public Dictionary<string, FundInfoResponse> funds;
            public object funds;
            // public UserRightsResponse rights;
            public object rights;
            public int transaction_count;
            public int open_orders;
            public long server_time;
        }

        internal class FundInfoResponse
        {
            public decimal value;
            public decimal inOrders;
        }

        internal class UserRightsResponse : Dictionary<string, UserRightResponse> { }

        internal class UserRightResponse
        {
            public bool info;
            public bool trade;
            public bool withdraw;
        }

        #endregion

        #region Public

        internal class AssetPairsResponse
        {
            public Dictionary<string, AssetPairsInfo> pairs;
        }

        internal class AssetPairsInfo
        {
            public int decimal_places;
            public int hidden;
            public decimal min_price;
            public decimal max_price;
            public decimal min_amount;
            public decimal fee;
        }

        internal class TickerData
        {
            public decimal avg;
            public decimal high;
            public decimal low;
            public decimal last;
            public decimal vol;
            public decimal vol_cur;
            public decimal buy;
            public decimal sell;
            public long updated;
        }

        #endregion
    }
}
