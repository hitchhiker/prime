using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Core.Exchange.Rates
{
    public class LatestPriceCoordinator
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        private readonly List<LatestPriceProvider> _providers = new List<LatestPriceProvider>();
        private readonly List<LatestPriceResult> _results = new List<LatestPriceResult>();
        private readonly object _resultsLock = new object();
        private readonly object _commonLock = new object();
        private static readonly Lazy<LatestPriceCoordinator> Lazy = new Lazy<LatestPriceCoordinator>(() => new LatestPriceCoordinator());
        public static LatestPriceCoordinator I => Lazy.Value; //there can be only one.
        private readonly int _timerInterval = 5000;
        private readonly SubscriberList<LatestPriceRequest> _subscriptions;
        public IMessenger Messenger => _messenger;

        private LatestPriceCoordinator()
        {
            _messenger.Register<ExchangeRateRequestVerifiedMessage>(this, ExchangeRateRequestVerified);
            _messenger.Register<LatestPriceResult>(this, ExchangeRateResultCollect);
            _subscriptions = new SubscriberList<LatestPriceRequest>(OnExistingSubscription, OnAddedSubscription, OnRemovedSubscription);
        }

        private void OnRemovedSubscription(LatestPriceRequest request)
        {
            SyncProviders();
        }

        private void OnAddedSubscription(LatestPriceRequest request)
        {
            request.Discover();
        }

        private void OnExistingSubscription(LatestPriceRequest request)
        {
            var c = Results().FirstOrDefault(x => Equals(x.Request, request));
            if (c != null)
                _messenger.Send(c);
        }

        private void ExchangeRateRequestVerified(ExchangeRateRequestVerifiedMessage m)
        {
            SyncProviders();
        }

        private void ExchangeRateResultCollect(LatestPriceResult result)
        {
            lock (_resultsLock)
            {
                var e = _results.FirstOrDefault(x => x.Pair.Equals(result.Pair) && (x.Provider.Id == result.Provider.Id || x.ProviderConversion?.Id == result.Provider.Id));
                if (e != null)
                    _results.Remove(e);
                _results.Add(result);
            }
        }

        public IReadOnlyList<LatestPriceResult> Results()
        {
            lock (_resultsLock)
                return _results.ToList();
        }

        private void SyncProviders()
        {
            lock (_commonLock)
            {
                var grouped = _subscriptions.GetSubscriptions().Where(x => x.IsVerified).GroupBy(x => x.Network).ToList();

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
                var erprov = new LatestPriceProvider(new LatestPriceProviderContext(prov, this) {PollingSpan = TimeSpan.FromMilliseconds(_timerInterval)});
                _providers.Add(erprov);
                return erprov;
            }
        }

        public LatestPriceRequest AddRequest(object subsriber, AssetPair pair, Network network = null)
        {
            lock (_commonLock)
            {
                return _subscriptions.Subscribe(subsriber, new LatestPriceRequest(pair, network));
            }
        }

        public void RemoveRequest(object subscriber, LatestPriceRequest request)
        {
            lock (_commonLock)
            {
                _subscriptions.Unsubscribe(subscriber, request);
                
                if (request.IsConverted && request.ConvertedOther != null)
                    _subscriptions.Unsubscribe(subscriber, request.ConvertedOther);
            }
        }

        public void RemoveRequest(object subscriber, AssetPair pair, Network network = null)
        {
            lock (_commonLock)
            {
                RemoveRequest(this, new LatestPriceRequest(pair, network));
            }
        }

        public IReadOnlyList<LatestPriceResult> Rates { get; set; }
    }
}