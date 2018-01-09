using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Gate
{
    internal class GateSchema
    {
        internal class VolumeResponse
        {
            public bool result;
            public VolumeEntry[] data;
        }

        internal class TickerResponse
        {
            public bool result;
            public decimal last;
            public decimal lowestAsk;
            public decimal highestBid;
            public decimal percentChange;
            public decimal baseVolume;
            public decimal quoteVolume;
            public decimal high24hr;
            public decimal low24hr;
        }

        internal class VolumeEntry
        {
            public int no;
            public decimal rate;
            public decimal vol_a;
            public decimal vol_b;
            public decimal rate_percent;
            public decimal supply;
            public string symbol;
            public string name;
            public string name_en;
            public string name_cn;
            public string pair;
            public string marketcap;
            public string plot;
            public string curr_a;
            public string curr_b;
            public string curr_suffix;
            public string trend;
        }

        internal class OrderBookResponse
        {
            public bool result;
            public decimal[][] bids;
            public decimal[][] asks;
        }
    }
}
