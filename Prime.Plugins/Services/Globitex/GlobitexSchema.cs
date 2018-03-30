using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Globitex
{
    internal class GlobitexSchema
    {
        #region Public

        internal class TimeResponse
        {
            public long timestamp;
        }

        internal class SymbolsResponse
        {
            public SymbolEntryResponse[] symbols;
        }

        internal class SymbolEntryResponse
        {
            public decimal priceIncrement;
            public decimal sizeIncrement;
            public decimal sizeMin;
            public string symbol;
            public string currency;
            public string commodity;
        }

        internal class AllTickersResponse
        {
            public TickerResponse[] instruments;
        }

        internal class TickerResponse
        {
            public string symbol;
            public decimal ask;
            public decimal bid;
            public decimal last;
            public decimal low;
            public decimal high;
            public decimal open;
            public decimal volume;
            public decimal volumeQuote;
            public long timestamp;
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
            public ErrorEntryResponse[] errors;
        }

        internal class ErrorEntryResponse
        {
            public int code;
            public string message;
            public string data;
        }

        internal class BalanceResponse
        {
            public AccountResponse[] accounts;
        }

        internal class AccountResponse
        {
            public string account;
            public bool main;
            public BalanceEntryResponse[] balance;
        }

        internal class BalanceEntryResponse
        {
            public string currency;
            public decimal available;
            public decimal reserved;
        }

        internal class NewOrderResponse
        {
            public NewOrderEntryResponse ExecutionReport;
        }

        internal class NewOrderEntryResponse
        {
            public string orderId;
            public string clientOrderId;
            public string orderStatus;
            public string symbol;
            public string side;
            public decimal price;
            public decimal quantity;
            public string type;
            public string timeInForce;
            public decimal? lastQuantity;
            public decimal? lastPrice;
            public decimal? leavesQuantity;
            public decimal? cumQuantity;
            public decimal? averagePrice;
            public long created;
            public string execReportType;
            public long timestamp;
            public string account;
            public string orderSource;
        }

        internal class ActiveOrdersResponse
        {
            public ActiveOrderEntryResponse[] orders;
        }

        internal class ActiveOrderEntryResponse
        {
            public string orderId;
            public string orderStatus;
            public long lastTimestamp;
            public decimal orderPrice;
            public decimal orderQuantity;
            public decimal avgPrice;
            public string type;
            public string timeInForce;
            public string clientOrderId;
            public string symbol;
            public string side;
            public string account;
            public string orderSource;
            public decimal leavesQuantity;
            public decimal cumQuantity;
            public decimal execQuantity;
        }

        #endregion
    }
}
