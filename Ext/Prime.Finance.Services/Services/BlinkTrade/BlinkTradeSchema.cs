using System;
using System.Collections.Generic;

namespace Prime.Finance.Services.Services.BlinkTrade
{
    internal class BlinkTradeSchema
    {
        #region Private
        internal class ErrorResponse
        {
            public string Status;
            public string Description;
        }

        internal class BalanceResponse
        {
            public long ClientID;
            public int BalanceReqID;
            public Dictionary<string, decimal> Available;
        }

        internal class OrderInfoResponse
        {
            public string OrderID;
            public string ExecID;
            public int ExecType;
            public int OrdStatus;
            public int CumQty;
            public string Symbol;
            public decimal OrderQty;
            public string LastShares;
            public string LastPx;
            public decimal Price;
            public int TimeInForce;
            public int LeavesQty;
            public string MsgType;
            public int ExecSide;
            public int OrdType;
            public int CxlQty;
            public int Side;
            public long ClOrdID;
            public decimal AvgPx;
        }

        internal class ActiveOrdersResponse
        {
            public OrderInfoResponse[] OrdListGrp;
            public int PageSize;
            public long OrdersReqID;
        }

        internal class WithdrawalResponse
        {
            public long WithdrawListReqID;
            public WithdrawalEntryResponse[] WithdrawListGrp;
        }

        internal class WithdrawalInfoEntryResponse
        {
            public string Wallet;
            public string Instant;
            public string Fees;
        }

        internal class WithdrawalEntryResponse
        {
            public int WithdrawID;
            public string Method;
            public string Currency;
            public decimal Amount;
            public WithdrawalInfoEntryResponse Data;
            public DateTime Created;
            public int Status;
            public string ReasonID;
            public string Reason;
            public decimal PercentFee;
            public decimal FixedFee;
            public decimal PaidAmount;
            public long UserID;
            public string Username;
            public int BrokerID;
            public string ClOrdID;
        }
        #endregion

        #region Public
        internal class TickerResponse : Dictionary<string, object> { }

        internal class OrderBookResponse
        {
            public string pair;
            public decimal[][] asks;
            public decimal[][] bids;
        }
        #endregion
    }
}
