using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public class MarketPricesResult
    {
        private AssetPairs _missedPairs;

        public MarketPricesResult()
        {
            MarketPrices = new List<MarketPrice>();
            _missedPairs = new AssetPairs();
        }

        public List<MarketPrice> MarketPrices { get; set; }
        public bool IsCompleted => MissedPairs.Count == 0;

        public AssetPairs MissedPairs { get; set; }
    }
}
