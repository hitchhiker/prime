using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Xbtce
{
    internal class XbtceSchema
    {
        internal class TickerResponse
        {
            public string Symbol;
            public decimal BestBid;
            public decimal BestAsk;
            public decimal LastBuyPrice;
            public decimal LastBuyVolume;
            public decimal LastSellPrice;
            public decimal LastSellVolume;
            public decimal DailyBestBuyPrice;
            public decimal DailyBestSellPrice;
            public decimal DailyTradedBuyVolume;
            public decimal DailyTradedSellVolume;
            public decimal DailyTradedTotalVolume;
            public long LastSellTimestamp;
            public long LastBuyTimestamp;
        }
    }
}
