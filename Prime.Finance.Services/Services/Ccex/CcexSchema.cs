using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Prime.Finance.Services.Services.Ccex
{
    internal class CcexSchema
    {
        #region Base

        internal class BaseResponse<T>
        {
            public bool success;
            public string message;
            public T result;
        }

        internal class ErrorResponse
        {
            public bool success;
            public string message;
        }

        #endregion

        #region Public

        internal class AllTickersResponse : Dictionary<string, TickerEntryResponse>
        {
        }

        internal class AssetPairsResponse
        {
            public string[] pairs;
        }

        internal class VolumeResponse
        {
            public bool success;
            public string message;
            public VolumeEntryResponse[] result;
        }

        internal class VolumeEntryResponse
        {
            public string MarketName;
            public decimal Volume;
            public decimal BaseVolume;
        }

        internal class TickerResponse
        {
            public TickerEntryResponse ticker;
        }

        internal class TickerEntryResponse
        {

            public decimal high;
            public decimal low;
            public decimal avg;
            public decimal lastbuy;
            public decimal lastsell;
            public decimal buy;
            public decimal sell;
            public decimal lastprice;
            public decimal buysupport;
            public long updated;
        }

        internal class OrderBookEntryResponse
        {
            public OrderBookItemResponse[] buy;
            public OrderBookItemResponse[] sell;
        }

        internal class OrderBookItemResponse
        {
            public decimal rate;
            public decimal quantity;
        }

        internal class OrderBookResponse
        {
            public bool success;
            public string message;
            public OrderBookEntryResponse result;
        }

        #endregion

        #region Private

        internal class BalancesEntryResponse
        {
            public object Currency;
            public decimal Balance;
            public decimal Available;
            public decimal Pending;
            public string CryptoAddress;
        }

        internal class BalancesResponse : BaseResponse<BalancesEntryResponse[]>
        {

        }

        internal class OrderLimitResponse : BaseResponse<OrderLimitEntryResponse>
        {
        }

        internal class OrderLimitEntryResponse
        {
            public string uuid;
        }

        internal class OrderInfoResponse
        {
            public string AccountId;
            public string OrderUuid;
            public string Exchange;
            public string Type;
            public decimal Quantity;
            public decimal QuantityRemaining;
            public decimal Limit;
            public decimal Reserved;
            public decimal ReserveRemaining;
            public decimal CommissionReserved;
            public decimal CommissionReserveRemaining;
            public decimal CommissionPaid;
            public decimal Price;
            public decimal? PricePerUnit;
            public DateTime Opened;
            public string Closed;
            public string IsOpen;
            public string Sentinel;
            public string CancelInitiated;
            public string ImmediateOrCancel;
            public string IsConditional;
            public string Condition;
            public string ConditionTarget;
        }

        #endregion
    }
}
