using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class MarketPriceAudit
    {
        public static TimeSpan FlushSpan = TimeSpan.FromHours(1);
        private readonly object _lock = new object();
        private readonly Dictionary<ObjectId, List<MarketPrice>> _prices = new Dictionary<ObjectId, List<MarketPrice>>();

        public void AddRange(IEnumerable<MarketPrice> prices)
        {
            foreach (var p in prices)
                Add(p);
        }

        public void Add(MarketPrice price)
        {
            lock (_lock)
            {
                if (!_prices.ContainsKey(price.Pair.Id))
                    _prices.Add(price.Pair.Id, new List<MarketPrice>());

                var col = _prices[price.Pair.Id];
                var dt = DateTime.UtcNow.Add(-FlushSpan);
                col.RemoveAll(x => x.UtcCreated < dt);
                col.Add(price);
            }
        }

        public IReadOnlyList<MarketPrice> Get(AssetPair pair)
        {
            lock (_lock)
                return _prices.ContainsKey(pair.Id) ? _prices[pair.Id].ToList() : new List<MarketPrice>();
        }

        public (TimeSpan duration, decimal percentage) PercentageDirection(Network network, AssetPair pair, TimeSpan span)
        {
            var prices = Get(pair).Where(x => x.Network.Id == network.Id).ToList();
            if (prices.Count < 2)
                return (TimeSpan.Zero, 0);

            var ordered = prices.Where(x => x.UtcCreated.IsWithinTheLast(span)).OrderBy(x=>x.UtcCreated).ToList();
            if (ordered.Count < 2)
                return (TimeSpan.Zero, 0);

            var earliest = ordered.First();
            var latest = ordered.Last();
            var duration = latest.UtcCreated - earliest.UtcCreated;
            return (duration, earliest.PercentageDifference(latest));
        }
    }
}