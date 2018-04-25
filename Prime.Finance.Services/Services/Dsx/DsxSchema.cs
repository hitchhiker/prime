using System.Collections.Generic;
using Newtonsoft.Json;
using RestEase;

namespace Prime.Finance.Services.Services.Dsx
{
    internal class DsxSchema
    {
        #region Public
        internal class TickerResponse : Dictionary<string, TickerEntryResponse>
        {

        }

        internal class AssetPairsResponse
        {
            public long server_time;
            public Dictionary<string, AssetPairEntryResponse> pairs;
        }

        internal class AssetPairEntryResponse
        {
            public int decimal_places;
            public int hidden;
            public int fee;
            public int amount_decimal_places;
            public decimal min_price;
            public decimal max_price;
            public decimal min_amount;
        }

        internal class TickerEntryResponse
        {
            public decimal high;
            public decimal low;
            public decimal last;
            public decimal buy;
            public decimal sell;
            public decimal avg;
            public decimal vol;
            public decimal vol_cur;
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

        #region Base
        internal class BaseResponse<T>
        {
            public int success;

            [JsonProperty("return")]
            public T returnObj;
        }

        internal class ErrorResponse
        {
            public int success;
            public string error;
        }
        #endregion

        #region Private

        internal class BalanceResponse : BaseResponse<BalanceInfoResponse>
        {

        }

        internal class BalanceInfoResponse
        {
            public Dictionary<string, BalanceEntryResponse> funds;
        }

        internal class BalanceEntryResponse
        {
            public decimal total;
            public decimal available;
        }

        internal class OrderResponse : BaseResponse<OrderInfoResponse>
        {

        }

        internal class OrderInfoResponse
        {
            public string orderId;
        }

        internal class OrderStatusResponse : BaseResponse<OrderStatusInfoResponse>
        {

        }

        internal class OrderStatusInfoResponse
        {
            public string pair;
            public string type;
            public decimal remainingVolume;
            public decimal volume;
            public decimal rate;
            public long timestampCreated;
            public int status;
            public string orderType;
        }

        internal class WithdrawalResponse : BaseResponse<WithdrawalInfoResponse>
        {

        }

        internal class WithdrawalInfoResponse
        {
            public string transactionId;
        }
        #endregion
    }
}
