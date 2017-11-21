using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime.Utility;

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

        public MarketPricesResult(IEnumerable<MarketPricesResult> prices) : this(prices.SelectMany(x=>x.MarketPrices)) { }

        public MarketPricesResult(IEnumerable<MarketPrice> prices)
        {
            MarketPrices = prices.AsList();
        }

        public List<MarketPrice> MarketPrices { get; set; } = new List<MarketPrice>();

        public MarketPrice FirstPrice => MarketPrices.FirstOrDefault();

        public bool IsCompleted => MissedPairs.Count == 0;

        public AssetPairs MissedPairs { get; set; } = new AssetPairs();

        public bool WasViaSingleMethod { get; set; }
    }
}
