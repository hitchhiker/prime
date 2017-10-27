using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Utility;

namespace Prime.Common
{
    public static class ExchangeExtensionMethods
    {/*
        private static readonly QueueSet<PriceCacheItem> PriceCache = new QueueSet<PriceCacheItem>(50);

        private static readonly object PriceCacheLock = new object();

        public static bool ExchangeHas(this IExchangeProvider provider, Asset asset)
        {
            return PublicContext.I.Data(provider).Assets.Any(x=>Equals(x, asset));
        }

        public static bool ExchangeHas(this IExchangeProvider provider, AssetPair pair)
        {
            return PublicContext.I.Data(provider).AssetPairs.Any(x => pair.Equals(x, true));
        }

        public static Money? Fx(this AssetPair pair, IPublicPriceProvider providerPreferred = null)
        {
            if (pair.Asset1.Equals(pair.Asset2))
                return new Money(1, pair.Asset1);

            if (providerPreferred!=null) { 
                var m = GetLatestPrice(providerPreferred, pair);
                if (m != null)
                    return m.Value;
            }

            var exs = Networks.I.ExchangeProviders;
            foreach (var e in exs.Where(x => x.ExchangeHas(pair)))
            {
                var m = GetLatestPrice(e, pair);
                if (m != null)
                    return m.Value;
            }

            var ppps = Networks.I.Providers.OfType<ILatestPriceAggregationProvider>().OfType<IPublicPriceProvider>();
            foreach (var e in ppps)
            {
                var m = GetLatestPrice(e, pair);
                if (m != null)
                    return m.Value;
            }

            return null;
        }

        public static Money? GetLatestPrice(this IPublicPriceProvider provider, AssetPair pair)
        {
            if (provider == null)
                throw new ArgumentException(nameof(provider) + " cannot be null for " + nameof(GetLatestPrice));

            lock (PriceCacheLock)
            {
                var e = PriceCache.FirstOrDefault(x => x.Match(provider, pair));
                if (e != null)
                    return e.IsMissing ? (Money?)null : e.Price;

                var m = ApiCoordinator.GetPrice(provider, new PublicPriceContext(pair)).Response?.Price;

                if (m == null || m == Money.Zero)
                {
                    PriceCache.Add(new PriceCacheItem() {Provider = provider, Pair = pair, IsMissing = true, UtcEntered = DateTime.UtcNow });
                    return null;
                }

                PriceCache.Add(new PriceCacheItem() { Provider = provider, Pair = pair, Price = m.Value, UtcEntered = DateTime.UtcNow });
                return m;
            }
        }*/
    }
}