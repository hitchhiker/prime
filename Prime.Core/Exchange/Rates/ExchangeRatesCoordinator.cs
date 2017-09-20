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
        private readonly List<ExchangeRateProvider> _runningProviders = new List<ExchangeRateProvider>();
        private static readonly Lazy<ExchangeRatesCoordinator> Lazy = new Lazy<ExchangeRatesCoordinator>(() => new ExchangeRatesCoordinator());
        public static ExchangeRatesCoordinator I => Lazy.Value; //there can be only one.

        private ExchangeRatesCoordinator()
        {
            TimerInterval = 5000;
            _messenger.Register<ExchangeRateRequestVerifiedMessage>(this, this, ExchangeRateRequestVerified);
        }
        
        public IMessenger Messenger => _messenger;

        private void ExchangeRateRequestVerified(ExchangeRateRequestVerifiedMessage m)
        {
            Start(UserContext.Current);
        }

        protected override void OnStart(UserContext context)
        {
            SyncProviders();
        }

        protected override void OnStop()
        {
            lock (StateLock)
            {
                _runningProviders.ForEach(x => x.Dispose());
                _runningProviders.Clear();
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
                    var prov = _runningProviders.FirstOrDefault(x => x.Network.Equals(g.Key)) ?? CreateProvider(g.Key);
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
                _runningProviders.Add(erprov);
                return erprov;
            }
        }

        public void AddRequest(AssetPair pair, Network network = null)
        {
            if (_requested.Any(x => x.Pair.Equals(pair) && Equals(network, x.Network)))
                return;

            _requested.Add(new ExchangeRateRequest(this, pair, network));
        }


        public IReadOnlyList<ExchangeRate> Rates { get; set; }

        public IReadOnlyList<ExchangeRateRequest> Requested => _requested;
    }
}