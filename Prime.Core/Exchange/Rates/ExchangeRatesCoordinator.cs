using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeRatesCoordinator : CoordinatorBase
    {
        private readonly UniqueList<ExchangeRateRequest> _requested = new UniqueList<ExchangeRateRequest>();
        private readonly List<ExchangeRateProvider> _providers = new List<ExchangeRateProvider>();
        private readonly List<ExchangeRateCollected> _results = new List<ExchangeRateCollected>();
        private readonly object _resultsLock = new object();
        private static readonly Lazy<ExchangeRatesCoordinator> Lazy = new Lazy<ExchangeRatesCoordinator>(() => new ExchangeRatesCoordinator());
        public static ExchangeRatesCoordinator I => Lazy.Value; //there can be only one.

        private ExchangeRatesCoordinator()
        {
            TimerInterval = 5000;
            _messenger.Register<ExchangeRateRequestVerifiedMessage>(this, ExchangeRateRequestVerified);
            _messenger.Register<ExchangeRateCollected>(this, ExchangeRateResultCollect);
        }
        
        public IMessenger Messenger => _messenger;

        private void ExchangeRateRequestVerified(ExchangeRateRequestVerifiedMessage m)
        {
            Start(UserContext.Current);
        }

        private void ExchangeRateResultCollect(ExchangeRateCollected result)
        {
            lock (_resultsLock)
            {
                var e = _results.FirstOrDefault(x => x.Pair.Equals(result.Pair) && x.Provider.Id == result.Provider.Id);
                if (e != null)
                    _results.Remove(e);
                _results.Add(result);
            }
        }

        public IReadOnlyList<ExchangeRateCollected> Results()
        {
            lock (_resultsLock)
                return _results.ToList();
        }

        protected override void OnStart(UserContext context)
        {
            SyncProviders();
        }

        protected override void OnStop()
        {
            lock (StateLock)
            {
                _providers.ForEach(x => x.Dispose());
                _providers.Clear();
            }
        }

        protected override void OnSubscribersChanged() { }

        private void SyncProviders()
        {
            lock (StateLock)
            {
                var grouped = _requested.Where(x => x.IsVerified).GroupBy(x => x.Network).ToList();
                foreach (var g in grouped)
                {
                    var prov = _providers.FirstOrDefault(x => x.Network.Equals(g.Key)) ?? CreateProvider(g.Key);
                    foreach (var r in g)
                        prov.AddVerifiedRequest(r);
                }
            }
        }

        private ExchangeRateProvider CreateProvider(Network network)
        {
            lock (StateLock)
            {
                var prov = network.PublicPriceProviders.FirstProvider();
                var erprov = new ExchangeRateProvider(new ExchangeRateProviderContext(prov, this) {PollingSpan = TimeSpan.FromMilliseconds(TimerInterval)});
                _providers.Add(erprov);
                return erprov;
            }
        }

        public void AddRequest(AssetPair pair, Network network = null)
        {
            if (_requested.Any(x => x.Pair.Equals(pair) && Equals(network, x.Network)))
                return;

            _requested.Add(new ExchangeRateRequest(this, pair, network));
        }


        public IReadOnlyList<ExchangeRateCollected> Rates { get; set; }

        public IReadOnlyList<ExchangeRateRequest> Requested => _requested;
    }
}