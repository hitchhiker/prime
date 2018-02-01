using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.BitBay
{
    internal class BitBaySchema
    {
        #region Public
        internal class TickerResponse
        {
            public decimal max;
            public decimal min;
            public decimal last;
            public decimal bid;
            public decimal ask;
            public decimal vwap;
            public decimal average;
            public decimal volume;
        }

        internal class OrderBookResponse
        {
            public decimal[][] bids;
            public decimal[][] asks;
        }
        #endregion

        #region Private
        internal class UserInfoResponse
        {
            public bool success;
            public decimal fee;
            public Dictionary<string, BalanceEntryResponse> balances;
        }

        internal class BalanceEntryResponse
        {
            public int available;
            public int locked;
        }
        #endregion
    }
}
