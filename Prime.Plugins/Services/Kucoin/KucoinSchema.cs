using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Kucoin
{
    internal class KucoinSchema
    {
        #region Base

        internal class BaseResponse<TResult>
        {
            public string msg;
            public string code;
            public bool success;
            public TResult data;
        }

        #endregion

        #region Public
        
        internal class TickerResponse
        {
            public string coinType;
            public bool trading;
            public decimal lastDealPrice;
            public decimal buy;
            public decimal sell;
            public string coinTypePair;
            public string symbol;
            public decimal sort;
            public decimal feeRate;
            public decimal volValue;
            public decimal high;
            public decimal datetime;
            public decimal vol;
            public decimal low;
            public decimal changeRate;
        }
        
        internal class OrderBookResponse
        {
            public decimal[][] SELL;
            public decimal[][] BUY;
        }

        #endregion
    }
}
