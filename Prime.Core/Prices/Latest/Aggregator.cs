using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Core.Prices.Latest.Messages;
using Prime.Utility;

namespace Prime.Core.Prices.Latest
{
    internal class Aggregator : IDisposable
    {
        private readonly Messenger _lpm;
        private readonly List<LatestPriceProvider> _providers = new List<LatestPriceProvider>();
        private readonly List<LatestPriceResultMessage> _results = new List<LatestPriceResultMessage>();
        private readonly object _resultsLock = new object();
        private readonly object _commonLock = new object();
        private readonly int _timerInterval = 5000;
        public IMessenger M { get; } = DefaultMessenger.I.Default;

        internal Aggregator(Messenger lpm)
        {
            _lpm = lpm;
            M.RegisterAsync<VerifiedMessage>(this, LatestPriceRequestVerifiedMessage);
            M.RegisterAsync<LatestPriceResultMessage>(this, LatestPriceResultMessage);
        }

        internal void LatestPriceRequestVerifiedMessage(VerifiedMessage m)
        {
            SyncProviders();
        }

        internal void LatestPriceResultMessage(LatestPriceResultMessage result)
        {
            lock (_resultsLock)
            {
                _results.RemoveAll(x => x.IsSimilarRequest(result));
                _results.Add(result);
            }
        }

        public IReadOnlyList<LatestPriceResultMessage> Results()
        {
            lock (_resultsLock)
                return _results.ToList();
        }

        public void SyncProviders()
        {
            lock (_commonLock)
            {
                var reqs = _lpm.GetRequests();
                var active = reqs.Where(x => x.IsDiscovered).Select(x=>x.Discovered).ToList();
                if (active.Count == 0)
                    return;

                var converted = active.Where(x => x.IsConverting).Select(x => x.ConversionOther).ToList();
                active.AddRange(converted);

                var grouped = GetPairsByProviderGrouped(active);

                var inuse = new List<LatestPriceProvider>();
                foreach (var g in grouped.Where(kv => kv.Key != null && kv.Any()))
                {
                    try
                    {
                        var prov = _providers.FirstOrDefault(x => x.Provider.Equals(g.Key)) ?? CreateProvider(g.Key);
                        prov.SyncPairRequests(g.Select(x => x.Pair));
                        inuse.Add(prov);
                    }
                    catch (Exception e)
                    {
                        Logging.I.DefaultLogger.Error($"Unable to sync {nameof(LatestPriceProvider)} for provider {g.Key.Title} {e.Message}");
                    }
                }

                //remove unused

                var unused = _providers.Except(inuse).ToList();
                foreach (var prov in unused)
                {
                    prov.Dispose();
                    _providers.Remove(prov);
                }
            }
        }

        private List<IGrouping<IPublicPricingProvider, AssetPairNetworks>> GetPairsByProviderGrouped(List<AssetPairNetworks> requests)
        {
            return requests.GroupBy(x => x.Provider<IPublicPricingProvider>()).ToList();
        }

        private LatestPriceProvider CreateProvider(IPublicPricingProvider provider)
        {
            lock (_commonLock)
            {
                var erprov = new LatestPriceProvider(new LatestPriceProviderContext(provider, this) {PollingSpan = TimeSpan.FromMilliseconds(_timerInterval)});
                _providers.Add(erprov);
                return erprov;
            }
        }
        
        public IReadOnlyList<LatestPriceResultMessage> Rates { get; set; }

        public Money? ConvertExisting(Money money, Asset toCurrency)
        {
            var pair = new AssetPair(money.Asset, toCurrency);
            var rate = Results().FirstOrDefault(x => x.Pair.Equals(pair));
            return rate == null ? (Money?) null : new Money((decimal) money * rate.Price, toCurrency);
        }

        public LatestPriceResultMessage GetResult(AssetPair pair)
        {
            return Results().FirstOrDefault(x => x.Pair.Equals(pair));
        }

        public void Dispose()
        {
            M.UnregisterAsync(this);
        }
    }
}