using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Liqui
{
    internal class LiquiSchema
    {
        internal class AssetPairsResponse
        {
            public long server_time;
            public Dictionary<string, AssetPairEntry> pairs;
        }

        internal class AllTickersResponse : Dictionary<string, TickerResponse>
        {
        }

        internal class OrderBookResponse : Dictionary<string, OrderBookResponse>
        {
        }

        internal class AssetPairEntry
        {
            public int decimal_places;
            public decimal min_price;
            public decimal max_price;
            public decimal min_amount;
            public decimal max_amount;
            public decimal min_total;
            public decimal hidden;
            public decimal fee;
        }

        internal class TickerResponse
        {
            public decimal high;
            public decimal low;
            public decimal avg;
            public decimal vol;
            public decimal vol_cur;
            public decimal last;
            public decimal buy;
            public decimal sell;
            public long updated;
        }

        internal class OrderBookEntryResponse
        {
            public decimal[][] asks;
            public decimal[][] bids;
        }
    }
}
