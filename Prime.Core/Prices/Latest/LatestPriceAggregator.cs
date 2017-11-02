using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.Utility;

namespace Prime.Common.Exchange.Rates
{
    public class LatestPriceAggregator : IDisposable
    {
        private readonly LatestPriceMessenger _lpm;
        private readonly List<LatestPriceProvider> _providers = new List<LatestPriceProvider>();
        private readonly List<LatestPriceResultMessage> _results = new List<LatestPriceResultMessage>();
        private readonly object _resultsLock = new object();
        private readonly object _commonLock = new object();
        private readonly int _timerInterval = 5000;
        public IMessenger M { get; } = DefaultMessenger.I.Default;

        internal LatestPriceAggregator(LatestPriceMessenger lpm)
        {
            _lpm = lpm;
            M.RegisterAsync<InternalLatestPriceRequestVerifiedMessage>(this, LatestPriceRequestVerifiedMessage);
            M.RegisterAsync<LatestPriceResultMessage>(this, LatestPriceResultMessage);
        }

        internal void LatestPriceRequestVerifiedMessage(InternalLatestPriceRequestVerifiedMessage m)
        {
            SyncProviders();
        }

        internal void LatestPriceResultMessage(LatestPriceResultMessage result)
        {
            lock (_resultsLock)
            {
                var e = _results.FirstOrDefault(x => x.Pair.Equals(result.Pair) && (x.Provider.Id == result.Provider.Id || x.ProviderConversion?.Id == result.Provider.Id));
                if (e != null)
                    _results.Remove(e);
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
                var subs = reqs.Where(x => x.IsVerified).ToList();
                var sr = subs.Where(x => x.IsConverted).Select(x => x.ConvertedOther).ToList();
                subs.AddRange(sr);
               
                var grouped = subs.GroupBy(x => x.Network).ToList();

                var inuse = new List<LatestPriceProvider>();
                foreach (var g in grouped)
                {
                    var prov = _providers.FirstOrDefault(x => x.Network.Equals(g.Key)) ?? CreateProvider(g.Key);
                    prov.SyncVerifiedRequests(g);
                    inuse.Add(prov);
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

        private LatestPriceProvider CreateProvider(Network network)
        {
            lock (_commonLock)
            {
                var prov = network.PublicPriceProviders.FirstProvider();
                if (prov == null)
                    throw new Exception($"Can't find {nameof(IPublicPriceSuper)} for {network.Name}");

                var erprov = new LatestPriceProvider(new LatestPriceProviderContext(prov, this) {PollingSpan = TimeSpan.FromMilliseconds(_timerInterval)});
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