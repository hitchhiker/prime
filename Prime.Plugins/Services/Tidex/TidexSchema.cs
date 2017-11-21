using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Tidex
{
    internal class TidexSchema
    {
        internal class AssetPairsResponse
        {
            public Dictionary<string, AssetPairsInfo> pairs;
        }

        internal class AssetPairsInfo
        {
            public int decimal_places;
            public int hidden;
            public decimal min_price;
            public decimal max_price;
            public decimal min_amount;
            public decimal fee;
        }

        internal class TickerData
        {
            public decimal avg;
            public decimal high;
            public decimal low;
            public decimal last;
            public decimal vol;
            public decimal vol_cur;
            public decimal buy;
            public decimal sell;
            public long updated;
        }
    }
}
