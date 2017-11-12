using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Utility;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public sealed class AssetPairNetworkRoutes : IEnumerable<ExchangeHop>
        {
            public readonly AssetPair Pair;

            public readonly IReadOnlyList<MarketPrice> Prices;
            private readonly List<ExchangeHop> _routes = new List<ExchangeHop>();
            public readonly ExchangeHop BestRoute;

            public decimal BestPercentage => BestRoute.Percentage;

            public AssetPairNetworkRoutes(AssetPair pair, List<MarketPrice> prices)
            {
                Pair = pair;
                Prices = prices;

                var networks = prices.Select(x => x.Network).ToUniqueList();
                foreach (var n1 in networks.Take(networks.Count-1))
                {
                    var i = networks.IndexOf(n1) + 1;

                    foreach (var n2 in networks.Skip(i).Take(networks.Count- i))
                    {
                        var p1 = prices.FirstOrDefault(x => Equals(x.Network, n1) && x.Pair.Equals(pair));
                        var p2 = prices.FirstOrDefault(x => Equals(x.Network, n2) && x.Pair.Equals(pair));
                        _routes.Add(new ExchangeHop(pair, p1, p2));
                    }
                }

                BestRoute = _routes.OrderByDescending(x => x.Percentage).FirstOrDefault();
            }

            public IEnumerator<ExchangeHop> GetEnumerator()
            {
                return _routes.GetEnumerator();
            }

            public override string ToString()
            {
                return $"{_routes.Count} route(s) -> Top: {BestRoute}";
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable) _routes).GetEnumerator();
            }
        }
    }
}