using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Common.Prices.Latest;
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

            _dN.BuildData(networkGraph.PairsByNetwork);

            var source = _dN.DeNormalise(NetworkGraph.PairsByNetworkRaw);

            _pricesByNetwork = PopulatePriceGraph(source).ToDictionary(x=>x.Key, y=>y.Value.AsReadOnlyList());

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

        private Dictionary<Network, List<MarketPrice>> PopulatePriceGraph(IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> source)
        {
            var result = new Dictionary<Network, List<MarketPrice>>();
            foreach (var kv in source)
            {
                if (NetworkContext.BadNetworks.Contains(kv.Key))
                    continue;

                var db = kv.Key.NameLowered != "###" ? PublicContext.I.As<MarketPricesData>().FirstOrDefault(x => x.Id == MarketPricesData.GetHash(kv.Key)) : null;

                if (db != null && !db.UtcCreated.IsWithinTheLast(TimeSpan.FromHours(1)))
                    db = null;

                if (PricesContext.FlushPrices)
                    db = null;

                var e = db ?? GrabPriceDataAndSave(kv);
                if (e == null)
                    continue;

                var normalised = e.Prices.ToList();

                normalised.RemoveAll(x => x.Price == 0);
                normalised.RemoveAll(x => !kv.Value.Any(a => a.EqualsOrReversed(x.Pair)));

                var forNormalisation = normalised.Where(x => !x.Pair.IsNormalised).ToList();
                normalised.RemoveAll(x => forNormalisation.Contains(x));

                foreach (var i in forNormalisation)
                    normalised.Add(i.Reversed);

                result.Add(kv.Key, normalised);

                Console.WriteLine($"Retrieved price data for {kv.Key.Name}: {normalised.Count} / {kv.Value.Count} pairs.");
            }

            return result;
        }

        private static MarketPricesData GrabPriceDataAndSave(KeyValuePair<Network, IReadOnlyList<AssetPair>> kv)
        {
            Console.WriteLine($"Getting price data for {kv.Key.Name} from {kv.Value.Count} pairs.");

            var prov = kv.Key.PublicPriceProviders.FirstDirectProvider<IPublicPricingProvider>();
            if (prov == null)
            {
                Console.WriteLine($"No provider found for {kv.Key.Name}");
                return null;
            }
            var r = ApiCoordinator.GetPricing(prov, new PublicPricesContext(kv.Value.ToList()));
            if (r.IsNull)
            {
                Console.WriteLine($"No prices found for {kv.Key.Name}");
                return null;
            }

            var data = new MarketPricesData(kv.Key, r.Response.MarketPrices);
            data.SavePublic();
            return data;
        }
    }
}