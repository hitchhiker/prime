using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BlinkTrade
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
            public string MsgType;
            public long ClientID;
            public int BalanceReqID;
            public Dictionary<string, decimal> Available;
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
