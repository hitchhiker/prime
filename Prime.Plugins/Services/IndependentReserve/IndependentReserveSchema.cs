using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.IndependentReserve
{
    internal class IndependentReserveSchema
    {
        internal class TickerResponse
        {
            public DateTime CreatedTimestampUtc;
            public decimal CurrentHighestBidPrice;
            public decimal CurrentLowestOfferPrice;
            public decimal DayAvgPrice;
            public decimal DayHighestPrice;
            public decimal DayLowestPrice;
            public decimal DayVolumeXbt;
            public decimal DayVolumeXbtInSecondaryCurrrency;
            public decimal LastPrice;
            public string PrimaryCurrencyCode;
            public string SecondaryCurrencyCode;
        }
    }
}
