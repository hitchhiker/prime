using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Yobit
{
    internal class YobitSchema
    {
        internal class AssetPairsResponse
        {
            public long server_time;
            public Dictionary<string, AssetPairEntryResponse> pairs;
        }

        internal class AllTickersResponse : Dictionary<string, TickerResponse>
        {
        }

        internal class AssetPairEntryResponse
        {
            public int decimal_places;
            public int hidden;
            public decimal min_price;
            public decimal max_price;
            public decimal min_amount;
            public decimal min_total;
            public decimal fee;
            public decimal fee_buyer;
            public decimal fee_seller;
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

        internal class OrderBookResponse : Dictionary<string, OrderBookEntryResponse>
        {
        }

        internal class OrderBookEntryResponse 
        {
            public decimal[][] asks;
            public decimal[][] bids;
        }
    }
}
