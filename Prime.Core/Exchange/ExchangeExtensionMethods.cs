using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Utility;

namespace Prime.Core
{
    public static class ExchangeExtensionMethods
    {
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

        public static IOhlcProvider GetOhlcProvider(this AssetPair pair)
        {
            var provs = Networks.I.OhlcProviders.OrderByDescending(x => x.Priority).ToList();
            return null;
        }

        public static Money? GetLatestPrice(this IExchangeProvider provider, AssetPair pair)
        {
            lock (PriceCacheLock)
            {
                var e = PriceCache.FirstOrDefault(x => x.Match(pair));
                if (e != null)
                    return e.Price;

                var r = AsyncContext.Run(() => provider.GetLastPrice(new PublicPriceContext(pair)));
                if (r == Money.Zero)
                    return null;

                PriceCache.Add(new PriceCacheItem() {Pair = pair, Price = r, UtcEntered = DateTime.UtcNow});
                return r;
            }
        }

        public static Money? GetLatestPrice(this IPublicPricesProvider provider, AssetPair pair)
        {
            lock (PriceCacheLock)
            {
                var e = PriceCache.FirstOrDefault(x => x.Match(pair));
                if (e != null)
                    return e.Price;

                var r = AsyncContext.Run(() => provider.GetLatestPrice(pair) ?? new Task<PriceLatest>(() => null));
                if (r != null)
                    PriceCache.Add(new PriceCacheItem() { Pair = pair, Price = r.Prices.FirstOrDefault(), UtcEntered = DateTime.UtcNow });

                return r?.Prices?.FirstOrDefault() ?? Money.Zero;
            }
        }

        public static Money? Fx(this AssetPair pair, INetworkProvider providerPreferred = null)
        {
            if (pair.Asset1.Equals(pair.Asset2))
                return new Money(1, pair.Asset1);

            var m = GetLatestPrice(providerPreferred, pair);
            if (m != null)
                return m.Value;

            var exs = Networks.I.ExchangeProviders;
            foreach (var e in exs.Where(x => x.ExchangeHas(pair)))
            {
                m = GetLatestPrice(e, pair);
                if (m != null)
                    return m.Value;
            }

            var ppps = Networks.I.Providers.OfType<ILatestPriceAggregationProvider>().ToList();
            foreach (var e in ppps)
            {
                m = GetLatestPrice(e, pair);
                if (m != null)
                    return m.Value;
            }

            return null;
        }

        private static Money? GetLatestPrice(INetworkProvider provider, AssetPair pair)
        {
            if (provider == null)
                return null;

            Money? m;

            var ppp = provider as IPublicPricesProvider;
            if (ppp != null)
            {
                try
                {
                    var p = AsyncContext.Run(() => ppp.GetLatestPrice(pair));
                    m = p?.Prices.FirstOrDefault(x => pair.Asset2.Equals(x.Asset));
                    return m;
                }
                catch { }
                return null;
            }

            var ep = provider as IExchangeProvider;
            if (ep == null)
                return null;

            try { 
                m = AsyncContext.Run(() => ep.GetLatestPrice(pair));
                return m;
            }
            catch { }
            return null;
        }
    }
}