using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Core
{
    public class AssetPairProviderDiscovery
    {
        private readonly PairProviderDiscoveryContext _context;
        private readonly AssetPair _pair;
        private const string IntermediariesCsv = "USD,BTC,EUR,LTC,USDT";
        private static readonly List<Asset> Intermediaries = IntermediariesCsv.ToCsv().Select(x => x.ToAssetRaw()).ToList();

        public AssetPairProviderDiscovery(PairProviderDiscoveryContext context)
        {
            _context = context;
            _pair = context.Pair;
        }

        public AssetPairKnownProviders Discover()
        {
            if (_pair == null)
                return null;

            return DiscoverSpecified() ?? DiscoverDirect() ?? DiscoverPegged() ?? DiscoverConverter();
        }

        private AssetPairKnownProviders DiscoverSpecified()
        {
            if (_context.Network == null)
                return null;

            return DiscoverSpecified(_pair, _context.Network, false) ?? (_context.ReversalEnabled ? DiscoverSpecified(_pair.Reverse(), _context.Network, true) : null);
        }

        private static AssetPairKnownProviders DiscoverSpecified(AssetPair pair, Network network, bool isReversed)
        {
            var provs = GetProviders(pair).Where(x => x.Network.Equals(network)).ToList();
            return provs.Any() ? new AssetPairKnownProviders(pair, provs, isReversed) : null;
        }

        private AssetPairKnownProviders DiscoverDirect()
        {
            return DiscoverDirect(_pair, false) ?? (_context.ReversalEnabled ? DiscoverDirect(_pair.Reverse(), true) : null);
        }

        private static AssetPairKnownProviders DiscoverDirect(AssetPair pair, bool isReversed)
        {
            var provs = GetProviders(pair);
            return provs.Any() ? new AssetPairKnownProviders(pair, provs, isReversed) : null;
        }

        private AssetPairKnownProviders DiscoverPegged()
        {
            if (!_context.PeggedEnabled)
                return null;

            return DiscoverPegged(_pair, false) ?? (_context.ReversalEnabled ? DiscoverPegged(_pair.Reverse(), true) : null);
        }

        private static AssetPairKnownProviders DiscoverPegged(AssetPair pair, bool isReversed)
        {
            // Try alternate / pegged variation

            foreach (var ap in pair.PeggedPairs)
            {
                var provs = GetProviders(ap);
                if (provs.Any())
                    return new AssetPairKnownProviders(ap, provs, isReversed) {IsPegged = true};
            }

            return null;
        }
        
        private AssetPairKnownProviders DiscoverConverter()
        {
            if (!_context.ConversionEnabled)
                return null;

            return DiscoverConverter(_pair, false) ?? (_context.ReversalEnabled ? DiscoverConverter(_pair.Reverse(), true) : null);
        }

        private static AssetPairKnownProviders DiscoverConverter(AssetPair pair, bool isReversed)
        {
            return Intermediaries.Select(a => DiscoverConverter(pair, a, isReversed)).FirstOrDefault(provs => provs != null);
        }

        private static AssetPairKnownProviders DiscoverConverter(AssetPair opair, Asset viaAsset, bool isReversed)
        {
            var pair = new AssetPair(opair.Asset1, viaAsset);
            var p1 = GetProviders(pair);

            if (!p1.Any())
                return null;

            var pair2 = new AssetPair(viaAsset, opair.Asset2);
            var p2 = GetProviders(pair2);

            if (!p2.Any())
                return null;

            return new AssetPairKnownProviders(pair, p1)
            {
                Via = new AssetPairKnownProviders(pair2, p2, isReversed) {IsIntermediary = true}
            };
        }

        private static List<IOhlcProvider> GetProviders(AssetPair pair)
        {
            var apd = PublicContext.I.PubData.GetAssetPairData(pair);
            return apd.Exchanges.Count == 0 ? new List<IOhlcProvider>() : apd.AllProviders.OfType<IOhlcProvider>().DistinctBy(x => x.Id).ToList();
        }
    }
}