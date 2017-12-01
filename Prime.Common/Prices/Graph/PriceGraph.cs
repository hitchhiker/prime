using System;
using System.Collections.Generic;
using System.Linq;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public class PriceGraph
    {
        private readonly GraphAssetNetworkDictionaries _dN = new GraphAssetNetworkDictionaries();

        public readonly NetworkContext NetworkContext;
        public readonly PricesContext PricesContext;
        public readonly NetworkGraph NetworkGraph;

        private readonly List<MarketPrice> _prices;
        private readonly Dictionary<Network, IReadOnlyList<MarketPrice>> _pricesByNetwork;
        private readonly Dictionary<AssetPair, IReadOnlyList<MarketPrice>> _pricesByPair;

        public IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> PairsByNetwork => _dN.PairsByNetwork;
        public IReadOnlyDictionary<AssetPair, IReadOnlyList<Network>> NetworksByPair => _dN.NetworksByPair;
        public IReadOnlyList<Asset> Assets => _dN.Assets;
        public IReadOnlyList<AssetPair> AssetPairs => _dN.AssetPairs;
        public IReadOnlyList<Network> Networks => _dN.Networks;

        public IReadOnlyDictionary<Network, IReadOnlyList<MarketPrice>> PricesByNetwork => _pricesByNetwork;
        public IReadOnlyDictionary<AssetPair, IReadOnlyList<MarketPrice>> PricesByPair => _pricesByPair;
        public IReadOnlyList<MarketPrice> Prices => _prices;

        public PriceGraph(PricesContext context, NetworkGraph networkGraph)
        {
            PricesContext = context;
            NetworkContext = networkGraph.Context;
            NetworkGraph = networkGraph;

            _dN.BuildData(networkGraph.PairsByNetworkN);

            var source = _dN.DeNormalise(NetworkGraph.PairsByNetworkRaw);

            _pricesByNetwork = PopulatePriceGraph(source).GroupBy(x => x.Network).ToDictionary(x => x.Key, y => y.AsReadOnlyList());

            RemoveEmptyPricing(_pricesByNetwork);

            _dN.BuildData(_pricesByNetwork.ToDictionary(x=>x.Key, y=>y.Value.Select(x=>x.Pair).AsReadOnlyList()));

            _prices = _pricesByNetwork.Values.SelectMany(x => x).ToList();
            _pricesByPair = _prices.GroupBy(x=>x.Pair).ToDictionary(x => x.Key, y => y.AsReadOnlyList());
        }

        private static void RemoveEmptyPricing(Dictionary<Network, IReadOnlyList<MarketPrice>> pbn)
        {
            var emptyNets = pbn.Where(x => x.Value.Count == 0).Select(x => x.Key).ToUniqueList();
            pbn.RemoveAll(x => emptyNets.Contains(x.Key));
        }

        private MarketPrices PopulatePriceGraph(IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> source)
        {
            var networks = new List<Network>();
            var results = new List<MarketPrice>();
            foreach (var kv in source)
            {
                if (NetworkContext.BadNetworks.Contains(kv.Key))
                    continue;

                var db = PricesContext.FlushPrices ? null : GetDbPrices(kv.Key);
                if (db != null)
                {
                    Console.WriteLine("Pricing retrieved from DB: " + kv.Key.Name);
                    results.AddRange(db);
                    continue;
                }
                networks.Add(kv.Key);
            }

            if (!networks.Any())
                return new MarketPrices(results);

            var ctx = new PricingProviderContext
            {
                UseDirect = true,
                AfterData = p =>
                {
                    p = p.AsNormalised();

                    if (p.Count>1)
                        Console.WriteLine("Pricing retrieved from BULK API: " + p.FirstOrDefault()?.Network?.Name + " [" + p.Count + "]");
                    else
                        Console.WriteLine("Pricing retrieved from Single API: " + p.FirstOrDefault()?.ToString());
                    return p;
                }
            };

            var prices = AsyncContext.Run(() => PricingProvider.I.GetAsync(networks, ctx));

            Save(prices);

            results.AddRange(prices);
            return new MarketPrices(results);
        }

        private MarketPrices GetDbPrices(Network network)
        {
            var fromDb = network.NameLowered != "###" ? PublicContext.I.As<MarketPrices>().FirstOrDefault(x => x.Id == MarketPrices.GetHash(network)) : null;

            if (fromDb != null && !fromDb.UtcCreated.IsWithinTheLast(TimeSpan.FromHours(1)))
                return null;

            return fromDb;
        }

        private void Save(MarketPrices prices)
        {
            foreach (var kv in prices.GroupBy(x => x.Network))
            {
                var p = new MarketPrices(kv);
                p.SavePublic();
            }
        }
    }
}