using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public class MarketPricesResult
    {
        public List<MarketPrice> MarketPrices { get; set; } = new List<MarketPrice>();

        public bool IsCompleted => MissedPairs.Count == 0;

        public AssetPairs MissedPairs { get; set; } = new AssetPairs();
    }
}
