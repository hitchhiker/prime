using System;
using System.Collections.Generic;

namespace Prime.Plugins.Services.Poloniex
{
    internal class PoloniexSchema
    {
        #region Base

        internal class ErrorResponse
        {
            public string error;
        }

        internal class SuccessResponse
        {
            public bool success;
        }

        #endregion

        #region Private

        internal class BalancesDetailedResponse : Dictionary<string, BalanceDetailedResponse> { }

        internal class DepositAddressesResponse : Dictionary<string, string> { }

        /// <summary>
        /// returnOrderTrades response model for all markets.
        /// </summary>
        internal class OpenMarketOrdersResponse : Dictionary<string, OpenOrdersResponse> { }

        internal class OpenOrdersResponse : List<OpenOrderResponse> { }

        internal class OpenOrderResponse
        {
            public long orderNumber;
            public string type;
            public decimal rate;
            public decimal amount;
            public decimal total;
        }
        
        /// <summary>
        /// ReturnTradeHistory response model for all markets.
        /// </summary>
        internal class MarketTradeOrdersResponse : Dictionary<string, TradeOrdersResponse> { }
        
        internal class TradeOrdersResponse : List<TradeOrderResponse> { }

        internal class TradeOrderResponse : OpenOrderResponse
        {
            public long globalTradeID;
            public long tradeID;
            public DateTime date;
            public decimal fee;
            public string category;
        }

        internal class OrderLimitResponse
        {
            public string orderNumber;
            public List<TradeResultResponse> resultingTrades;
        }

        internal class OrderStatusResponse : List<TradeResultFinalResponse>
        {
        }

        internal class TradeResultResponse
        {
            public decimal amount;
            public DateTime date;
            public decimal rate;
            public decimal total;
            public string tradeID;
            public string type;
        }

        internal class TradeResultFinalResponse : TradeResultResponse
        {
            public decimal fee;
            public string currencyPair;
            public long globalTradeID;
        }

        internal class BalanceDetailedResponse
        {
            public decimal available;
            public decimal onOrders;
            public decimal btcValue;
        }

        internal class CancelOrderResponse : SuccessResponse { }

        #endregion

        #region Public

        internal class TickerResponse : Dictionary<string, TickerEntryResponse> { }

        internal class ChartEntriesResponse : List<ChartEntryResponse> { }

        internal class VolumeResponse : Dictionary<string, object> { }

        internal class OrderBookResponse
        {
            public decimal[][] asks;
            public decimal[][] bids;
            public int isFrozen;
            public int seq;
        }

        internal class ChartEntryResponse
        {
            public long date;
            public decimal high;
            public decimal low;
            public decimal open;
            public decimal close;
            public decimal volume;
            public decimal quoteVolume;
            public decimal weightedAverage;
        }

        internal class TickerEntryResponse
        {
            public int id;
            public decimal last;
            public decimal lowestAsk;
            public decimal highestBid;
            public decimal percentChange;
            public decimal baseVolume;
            public decimal quoteVolume;
            public string isFrozen;
            public decimal high24hr;
            public decimal low24hr;
        }

        #endregion

    }
}
