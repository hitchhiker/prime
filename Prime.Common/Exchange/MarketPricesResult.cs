using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prime.Common
{
    public class MarketPricesResult
    {
        public MarketPricesResult() { }

        public MarketPricesResult(MarketPrice price)
        {
            if (price != null)
                MarketPrices.Add(price);

            WasViaSingleMethod = true;
        }

        public MarketPricesResult(List<MarketPrice> prices)
        {
            MarketPrices = prices;
        }

        public List<MarketPrice> MarketPrices { get; set; } = new List<MarketPrice>();

        public MarketPrice FirstPrice => MarketPrices.FirstOrDefault();

        public bool IsCompleted => MissedPairs.Count == 0;

        public AssetPairs MissedPairs { get; set; } = new AssetPairs();

        public bool WasViaSingleMethod { get; set; }
    }
}
