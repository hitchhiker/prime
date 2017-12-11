using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Ccex
{
    internal class CcexSchema
    {
        internal class AllTickersResponse : Dictionary<string, TickerEntryResponse>
        {
        }

        internal class AssetPairsResponse
        {
            public string[] pairs;
        }

        internal class VolumeResponse
        {
            public bool success;
            public string message;
            public VolumeEntryResponse[] result;
        }

        internal class VolumeEntryResponse
        {
            public string MarketName;
            public decimal Volume;
            public decimal BaseVolume;
        }

        internal class TickerResponse
        {
            public TickerEntryResponse ticker;
        }

        internal class TickerEntryResponse
        {

            public decimal high;
            public decimal low;
            public decimal avg;
            public decimal lastbuy;
            public decimal lastsell;
            public decimal buy;
            public decimal sell;
            public decimal lastprice;
            public decimal buysupport;
            public long updated;
        }
    }
}
