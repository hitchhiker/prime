using System.Collections.Generic;

namespace Prime.Finance.Services.Services.Coinfloor
{
    internal class CoinfloorSchema
    {
        #region Base
        internal class ErrorResponse
        {
            public string error_code;
            public string error_msg;
        }

        #endregion

        #region Public
        internal class TickerResponse
        {
            public decimal last;
            public decimal high;
            public decimal low;
            public decimal vwap;
            public decimal volume;
            public decimal bid;
            public decimal ask;
        }

        internal class OrderBookResponse
        {
            public decimal[][] bids;
            public decimal[][] asks;
        }
        #endregion

        #region Private
        internal class BalancesResponse
        {
            public decimal xbt_balance;
            public decimal gbp_balance;
            public decimal xbt_reserved;
            public decimal gbp_reserved;
            public decimal xbt_available;
            public decimal gbp_available;
        }

        internal class OrderResponse
        {
            public string id;
            public int type;
            public decimal price;
            public decimal amount;
        }
        
        #endregion
    }
}
