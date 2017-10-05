using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeRatesCoordinator
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        private readonly UniqueList<ExchangeRateRequest> _requested = new UniqueList<ExchangeRateRequest>();
        private readonly List<ExchangeRateProvider> _providers = new List<ExchangeRateProvider>();
        private readonly List<ExchangeRateCollected> _results = new List<ExchangeRateCollected>();
        private readonly object _resultsLock = new object();
        private readonly object _commonLock = new object();
        private static readonly Lazy<ExchangeRatesCoordinator> Lazy = new Lazy<ExchangeRatesCoordinator>(() => new ExchangeRatesCoordinator());
        public static ExchangeRatesCoordinator I => Lazy.Value; //there can be only one.
        private readonly int _timerInterval = 5000;

        private ExchangeRatesCoordinator()
        {
            _messenger.Register<ExchangeRateRequestVerifiedMessage>(this, ExchangeRateRequestVerified);
            _messenger.Register<ExchangeRateCollected>(this, ExchangeRateResultCollect);
        }
        
        public IMessenger Messenger => _messenger;

        private void ExchangeRateRequestVerified(ExchangeRateRequestVerifiedMessage m)
        {
            lock (_commonLock)
                _requested.Add(m.Request);
            
            SyncProviders();
        }

        private void ExchangeRateResultCollect(ExchangeRateCollected result)
        {
            lock (_resultsLock)
            {
                var e = _results.FirstOrDefault(x => x.Pair.Equals(result.Pair) && (x.Provider.Id == result.Provider.Id || x.ProviderConversion?.Id == result.Provider.Id));
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

        private void SyncProviders()
        {
            lock (_commonLock)
            {
                var grouped = _requested.Where(x => x.IsVerified).GroupBy(x => x.Network).ToList();

                var inuse = new List<ExchangeRateProvider>();
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

        private ExchangeRateProvider CreateProvider(Network network)
        {
            lock (_commonLock)
            {
                var prov = network.PublicPriceProviders.FirstProvider();
                var erprov = new ExchangeRateProvider(new ExchangeRateProviderContext(prov, this) {PollingSpan = TimeSpan.FromMilliseconds(_timerInterval)});
                _providers.Add(erprov);
                return erprov;
            }
        }

        public ExchangeRateRequest AddRequest(AssetPair pair, Network network = null)
        {
            lock (_commonLock)
            {
                var err = new ExchangeRateRequest(this, pair, network);
                var exr = _requested.FirstOrDefault(x => x.Equals(err));
                if (exr != null)
                {
                    var c = Results().FirstOrDefault(x => Equals(x.Request, exr));
                    if (c != null)
                        _messenger.Send(c);
                    return exr;
                }

                err.Discover();
                return err;
            }
        }

        public void RemoveRequest(ExchangeRateRequest request)
        {
            lock (_commonLock)
            {
               if (request == null)
                    return;

                _requested.Remove(request);

                if (request.IsConverted && request.ConvertedOther != null)
                    _requested.Remove(request.ConvertedOther);

                SyncProviders();
            }
        }

        public void RemoveRequest(AssetPair pair, Network network = null)
        {
            lock (_commonLock)
            {
                var e = _requested.FirstOrDefault(x => x.Pair.Equals(pair) && (Equals(network, x.NetworkSuggested) || (network == null && x.NetworkSuggested == null)));
                if (e == null)
                    return;

                RemoveRequest(e);
            }
        }

        public IReadOnlyList<ExchangeRateCollected> Rates { get; set; }

        public IReadOnlyList<ExchangeRateRequest> Requested => _requested;
    }
}