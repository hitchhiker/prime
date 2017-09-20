using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Core
{
    public class AssetPairProviderDiscovery
    {
        private readonly PairProviderDiscoveryContext _context;
        private readonly AssetPair _pair;

        public AssetPairProviderDiscovery(PairProviderDiscoveryContext context)
        {
            _context = context;
            _pair = context.Pair;
        }

        public AssetPairKnownProviders FindProviders()
        {
            if (_pair == null)
                return null;

            var pub = PublicContext.I.PubData;

            // Return supplied network if valid

            if (_context.Network != null)
            {
                var provs = GetProviders(_pair).Where(x => x.Network.Equals(_context.Network)).ToList();
                if (provs.Any())
                    return new AssetPairKnownProviders() { Providers = provs, Pair = _pair };
            }

            // Find direct / no conversion provider

            var provsd = GetProviders(pub, _pair);
            if (provsd.Any())
                return new AssetPairKnownProviders { Providers = provsd, Pair = _pair };

            // Find direct via pegged

            if (_context.PeggedEnabled)
            {
                var provs = DiscoverPegged(pub);
                if (provs != null)
                    return provs;
            }

            if (_context.ConversionEnabled)
                return DiscoverConverter(pub);
            
            return null;
        }

        private AssetPairKnownProviders DiscoverPegged(PublicData pub)
        {
            // Try alternate / pegged variation

            foreach (var ap in _pair.PeggedPairs)
            {
                var p = GetProvider(pub, ap);
                if (p != null)
                    return new AssetPairKnownProviders() {Provider = p, Pair = ap, IsPegged = true};
            }

            return null;
        }

        private const string IntermediariesCsv = "USD,BTC,EUR,LTC,USDT";
        private static readonly List<Asset> Intermediaries = IntermediariesCsv.ToCsv().Select(x => x.ToAssetRaw()).ToList();

        private AssetPairKnownProviders DiscoverConverter(PublicData pub)
        {
            return Intermediaries.Select(a => DiscoverConverter(pub, a)).FirstOrDefault(provs => provs != null);
        }

        private AssetPairKnownProviders DiscoverConverter(PublicData pub, Asset viaAsset)
        {
            var pair = new AssetPair(_pair.Asset1, viaAsset);
            var p1 = GetProviders(pub, pair);

            if (!p1.Any())
                return null;

            var pair2 = new AssetPair(viaAsset, _pair.Asset2);
            var p2 = GetProviders(pub, pair2);

            if (!p2.Any())
                return null;

            return new AssetPairKnownProviders
            {
                Providers = p1,
                Pair = pair,
                Via = new AssetPairKnownProviders {Providers = p2, Pair = pair2, IsIntermediary = true}
            };
        }

        public static IList<IOhlcProvider> GetProviders(AssetPair pair)
        {
            return GetProviders(PublicContext.I.PubData, pair);
        }

        private static IOhlcProvider GetProvider(PublicData pub, AssetPair pair)
        {
            return GetProviders(pub, pair).FirstProvider();
        }

        private static List<IOhlcProvider> GetProviders(PublicData pub, AssetPair pair)
        {
            var who = pub.AssetExchangeData(pair);
            return who.Exchanges.Count == 0 ? new List<IOhlcProvider>() : who.AllProviders.OfType<IOhlcProvider>().DistinctBy(x => x.Id).ToList();
        }
    }
}