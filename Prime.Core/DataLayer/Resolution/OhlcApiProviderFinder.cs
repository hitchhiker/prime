using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Core
{
    public class OhlcApiProviderFinder
    {
        private readonly OhlcProviderFinderContext _context;
        private readonly AssetPair _pair;

        public OhlcApiProviderFinder(OhlcProviderFinderContext context)
        {
            _context = context;
            _pair = context.Pair;
        }

        public (OhlcPairProviders direct, OhlcPairProviders via) FindProvider()
        {
            if (_pair == null)
                return (null, null);

            var pub = PublicContext.I.PubData;

            // Find direct / no conversion provider

            var p = GetProvider(pub, _pair);
            if (p != null)
                return (new OhlcPairProviders() { Provider = p, Pair = _pair }, null);

            if (_context.PeggedEnabled)
            {
                // Try alternate / pegged variation
                foreach (var ap in _pair.PeggedPairs)
                {
                    p = GetProvider(pub, ap);
                    if (p != null)
                        return (new OhlcPairProviders() { Provider = p, Pair = ap, IsPegged = true }, null);
                }
            }

            if (_context.ConversionEnabled)
            {
                // Via BTC conversion

                var pair = new AssetPair(_pair.Asset1, Asset.Btc);
                var p1 = GetProviders(pub, pair);

                var pair2 = new AssetPair(Asset.Btc, _pair.Asset2);
                var p2 = GetProviders(pub, pair2);

                return (new OhlcPairProviders { Providers = p1, Pair = pair }, new OhlcPairProviders { Providers = p2, Pair = pair2, IsIntermediary = true });
            }

            return (null, null);
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