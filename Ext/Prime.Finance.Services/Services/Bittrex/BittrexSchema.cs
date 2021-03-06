﻿using System;
using System.Collections.Generic;

namespace Prime.Finance.Services.Services.Bittrex
{
    internal class BittrexSchema
    {
        #region Base

        internal class ResultResponse
        {
            public bool success;
            public string message;
        }

        internal class BaseResponse<TResult> : ResultResponse
        {
            public TResult result;
        }

        #endregion

        #region Private

        internal class BalancesResponse : BaseResponse<IList<BalanceResponse>> { }

        internal class DepositAddressResponse : BaseResponse<DepositAddressContainerResponse> { }

        internal class UuidResponse : BaseResponse<UuidEntry> { }

        internal class OpenOrdersResponse : BaseResponse<List<OpenOrderEntry>> { }

        internal class OrderResponse : BaseResponse<OrderEntry> { }

        internal class OrderHistoryResponse : BaseResponse<List<OrderHistoryEntry>> { }

        internal class WithdrawalResponse : BaseResponse<UuidEntry> { }

        internal class UuidEntry
        {
            public string uuid;
        }

        internal class OrderCommonBase
        {
            public string OrderUuid;
            public string Exchange;
            public decimal Quantity;
            public decimal QuantityRemaining;
            public decimal Limit;
            public decimal Price;
            public decimal? PricePerUnit;
            public bool IsConditional;
            public string Condition;
            public string ConditionTarget;
            public bool ImmediateOrCancel;

            public string OrderType;
        }

        internal class OpenOrderEntry : OrderCommonBase
        {
            public string Type => OrderType;

            public string Uuid;
            public decimal CommissionPaid;
            public DateTime? Opened;
            public DateTime? Closed;
            public bool CancelInitiated;
        }

        internal class OrderEntry : OrderCommonBase
        {
            public string Type => OrderType;

            public string AccountId;
            public decimal CommissionPaid;
            public decimal CommissionReserved;
            public decimal CommissionReservedRemaining;
            public bool IsOpen;
            public string Sentinel;
            public DateTime? Opened;
            public DateTime? Closed;
            public bool CancelInitiated;
        }

        internal class OrderHistoryEntry : OrderCommonBase
        {
            public string Type => OrderType;
            // public string OrderType;
            public string TimeStamp;
            public decimal Commission;
        }

        internal class BalanceResponse
        {
            public string Currency;
            public decimal Balance;
            public decimal Available;
            public decimal Pending;
            public string CryptoAddress;
            public bool Requested;
            public string Uuid;
        }

        internal class DepositAddressContainerResponse
        {
            public string Currency;
            public string Address;
        }



        #endregion

        #region Public

        internal class MarketEntriesResponse : BaseResponse<IList<MarketEntryResponse>> { }

        internal class TickerResponse : BaseResponse<TickerContainerResponse> { }

        internal class OrderBookResponse : BaseResponse<OrderBookDataResponse> { }

        internal class MarketSummariesResponse : BaseResponse<IList<MarketSummaryEntryResponse>> { }

        internal class MarketSummaryEntryResponse
        {
            public string MarketName;
            public decimal High;
            public decimal Low;
            public decimal Volume;
            public decimal Last;
            public decimal BaseVolume;
            public DateTime TimeStamp;
            public decimal Bid;
            public decimal Ask;
            public int OpenBuyOrders;
            public int OpenSellOrders;
            public decimal PrevDay;
            public DateTime Created;
            public object DisplayMarketName;
        }

        internal class OrderBookDataResponse
        {
            public OrderBookEntryResponse[] buy;
            public OrderBookEntryResponse[] sell;
        }

        internal class OrderBookEntryResponse
        {
            public decimal Quantity;
            public decimal Rate;
        }

        internal class TickerContainerResponse
        {
            public decimal Bid;
            public decimal Ask;
            public decimal Last;
        }

        internal class MarketEntryResponse
        {
            public string MarketCurrency;
            public string BaseCurrency;
            public string Litecoin;
            public string Bitcoin;
            public decimal MinTradeSize;
            public string MarketName;
            public bool IsActive;
            public DateTime Created;
        }

        #endregion

    }
}
