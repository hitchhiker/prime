using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Wex
{
    internal class WexSchema
    {
        internal class TickerResponse : Dictionary<string, TickerEntryResponse>
        {
        }

        internal class OrderBookResponse : Dictionary<string, OrderBookEntryResponse>
        {
        }

        internal class AllAssetsResponse
        {
            public long server_time;
            public Dictionary<string, AssetPairResponse> pairs;
        }

        internal class AssetPairResponse
        {
            public int decimal_places;
            public int hidden;
            public decimal min_price;
            public decimal max_price;
            public decimal min_amount;
            public decimal fee;
        }

        internal class TickerEntryResponse
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
