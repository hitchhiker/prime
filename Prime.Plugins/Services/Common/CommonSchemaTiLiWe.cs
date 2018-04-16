﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Prime.Plugins.Services.Common
{
    /// <summary>
    /// Contains API schema models for Liqui and Tidex exchanges.
    /// </summary>
    public class CommonSchemaTiLiWe
    {
        #region Base

        public class BaseResponse<T>
        {
            public byte success;

            [JsonProperty("return")]
            public T return_;

            public string error;
            public StatResponse stat;
        }

        public class StatResponse
        {
            public bool isSuccess;
            public string serverTime;
            public string time;
            public string errors;
        }

        public class BaseUserInfoDataResponse
        {
            public int transaction_count;
            public int open_orders;
            public long server_time;
        }

        #endregion

        #region Private

        public class UserInfoResponse : BaseResponse<UserInfoDataResponse> { }

        public class UserInfoDataResponse : BaseUserInfoDataResponse
        {
            public Dictionary<string, decimal> funds;
            public UserRightResponse rights;
        }

        public class UserInfoExtResponse : BaseResponse<UserInfoDataExtResponse> { }

        public class UserInfoDataExtResponse : BaseUserInfoDataResponse
        {
            public Dictionary<string, FundInfoResponse> funds;
            public UserRightResponse rights;
        }

        public class FundInfoResponse
        {
            public decimal value;
            public decimal inOrders;
        }

        public class UserRightResponse
        {
            public bool info;
            public bool trade;
            public bool withdraw;
        }

        public class OrderInfoResponse : BaseResponse<OrderInfoDataResponse> { }

        public class OrderInfoDataResponse : Dictionary<string, OrderInfoDataEntryResponse> { }

        public class OrderInfoDataEntryResponse
        {
            public string pair;
            public string type;
            public decimal start_amount;
            public decimal amount;
            public decimal rate;
            public long timestamp_created;
            public short status;
        }

        public class TradeResponse : BaseResponse<TradeDataResponse> { }

        public class TradeDataResponse
        {
            public decimal received;
            public decimal remains;
            public long order_id;
            public Dictionary<string, decimal> funds;
        }

        public class ActiveOrdersResponse : BaseResponse<Dictionary<string, ActiveOrders>> { }

        public class ActiveOrders
        {
            public string pair;
            public string type;
            public decimal start_amount;
            public decimal amount;
            public decimal rate;
            public long timestamp_created;
            public byte status;
        }

        #endregion

        #region Public

        public class AssetPairsResponse
        {
            public Dictionary<string, AssetPairsInfo> pairs;
        }

        public class OrderBookResponse : Dictionary<string, OrderBookEntryResponse>
        {
        }

        public class AssetPairsInfo
        {
            public int decimal_places;
            public int hidden;
            public decimal min_price;
            public decimal max_price;
            public decimal min_amount;
            public decimal fee;
        }

        public class TickerData
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

        public class OrderBookEntryResponse
        {
            public decimal[][] asks;
            public decimal[][] bids;
        }

        #endregion
    }
}