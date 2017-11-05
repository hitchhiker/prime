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
            IsCompleted = true;
            _missedPairs = new AssetPairs();
        }

        public void AddMissedPair(AssetPair pair)
        {
            _missedPairs.Add(pair);

            IsCompleted = false;
        }

        public List<MarketPrice> MarketPrices { get; set; }
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Gets missed pairs. Should not be used to add new pairs.
        /// </summary>
        public AssetPairs MissedPairs => _missedPairs;
    }
}
