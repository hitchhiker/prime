using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Prime.Plugins.Services.Yobit
{
    internal class YobitSchema
    {
        #region Public

        internal class AssetPairsResponse
        {
            public long server_time;
            public Dictionary<string, AssetPairEntryResponse> pairs;
        }

        internal class AllTickersResponse : Dictionary<string, TickerResponse>
        {
        }

        internal class AssetPairEntryResponse
        {
            public int decimal_places;
            public int hidden;
            public decimal min_price;
            public decimal max_price;
            public decimal min_amount;
            public decimal min_total;
            public decimal fee;
            public decimal fee_buyer;
            public decimal fee_seller;
        }

        internal class TickerResponse
        {
            public decimal high;
            public decimal low;
            public decimal avg;
            public decimal vol;
            public decimal vol_cur;
            public decimal last;
            public decimal buy;
            public decimal sell;
            public long updated;
        }

        internal class OrderBookResponse : Dictionary<string, OrderBookEntryResponse>
        {
        }

        internal class OrderBookEntryResponse
        {
            public decimal[][] asks;
            public decimal[][] bids;
        }

        #endregion

        #region Private

        internal class BaseResponse<T>
        {
            public bool success;
            [JsonProperty("return")]
            public T returnData;
        }

        internal class UserInfoResponse : BaseResponse<UserInfoEntryResponse>
        {
            public int transaction_count;
            public int open_orders;
            public long server_time;
        }

        internal class UserInfoEntryResponse
        {
            public Dictionary<string, decimal> funds;
            public Dictionary<string, decimal> funds_incl_orders;
            public RightsEntryResponse rights;
        }

        internal class RightsEntryResponse
        {
            public bool info;
            public bool trade;
            public bool withdraw;
        }

        internal class NewOrderResponse : BaseResponse<NewOrderEntryResponse>
        {
        }

        internal class NewOrderEntryResponse
        {
            public decimal received;
            public decimal remains;
            public string order_id;
            public Dictionary<string, decimal> funds;
        }

        internal class WithdrawalRequestResponse : BaseResponse<WithdrawalRequestEntryResponse>
        {

        }

        internal class WithdrawalRequestEntryResponse
        {
            public long server_time;
        }

        internal class ActiveOrdersResponse : BaseResponse<Dictionary<string, OrderInfoEntryResponse>>
        {

        }

        internal class OrderInfoResponse : BaseResponse<KeyValuePair<string, OrderInfoEntryResponse>>
        {

        }

        internal class OrderInfoEntryResponse
        {
            public string pair;
            public string type;
            public decimal start_amount;
            public decimal amount;
            public decimal rate;
            public long timestamp_created;
            public int status;
        }
        #endregion
    }
}
