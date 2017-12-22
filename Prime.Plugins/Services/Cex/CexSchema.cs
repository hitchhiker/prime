using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Cex
{
    internal class CexSchema
    {
        #region Base

        internal class BaseResponse
        {
            public string e;
            public string ok;
        }

        #endregion

        #region Private

        internal class OrderBookResponse
        {
            public long timestamp;
            public decimal[][] bids;
            public decimal[][] asks;
            public string pair;
            public int id;
            public decimal sell_total;
            public decimal buy_total;
        }

        #endregion

        #region Public

        internal class TickersResponse : BaseResponse
        {
            public TickerResponse[] data;
        }

        internal class TickerResponse
        {
            public long timestamp;
            public string pair;
            public decimal low;
            public decimal high;
            public decimal last;
            public decimal volume;
            public decimal volume30d;
            public decimal bid;
            public decimal ask;
        }

        internal class LatestPricesResponse : BaseResponse
        {
            public LatestPriceResponse[] data;
        }

        internal class LatestPriceResponse
        {
            public string symbol1;
            public string symbol2;
            public decimal lprice;
        }

        #endregion
    }
}
