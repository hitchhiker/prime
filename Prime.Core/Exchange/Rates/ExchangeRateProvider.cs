using System;
using System.Collections.Generic;
using System.Timers;
using System.Web.UI;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;
using System.Linq;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeRateProvider : IDisposable
    {
        private readonly ExchangeRateProviderContext _context;
        private readonly IPublicPriceProvider _provider;
        private readonly UniqueList<ExchangeRateRequest> _requesting = new UniqueList<ExchangeRateRequest>();
        private readonly List<AssetPairNormalised> _pairs = new List<AssetPairNormalised>();
        private readonly ExchangeRatesCoordinator _coordinator;
        private readonly IMessenger _messenger;
        private readonly Timer _timer;
        public readonly Network Network;
        private readonly object _timerLock = new object();

        public ExchangeRateProvider(ExchangeRateProviderContext context)
        {
            _context = context;
            _provider = context.Provider;
            Network = _provider.Network;
            _coordinator = context.Coordinator;
            _messenger = context.Coordinator.Messenger;
            _timer = new Timer(10) {AutoReset = false};
            _timer.Elapsed += delegate { UpdateTick(); };
            _timer.Start();
        }

        private DateTime _utcLastUpdate = DateTime.MinValue;
        protected void UpdateTick()
        {
            lock (_timerLock)
            {
                if (_pairs.Count!=0 && !_utcLastUpdate.IsWithinTheLast(_context.PollingSpan))
                {
                    _utcLastUpdate = DateTime.UtcNow;
                    Update();
                }

                if (!_isDisposed)
                    _timer.Start();
            }
        }

        public void ManualUpdate()
        {
            lock (_timerLock)
            {
                _utcLastUpdate = DateTime.UtcNow;
                Update();
            }
        }

        public bool IsFailing { get; private set; }

        public void AddVerifiedRequest(ExchangeRateRequest request)
        {
            if (!request.IsVerified)
                throw new ArgumentException($"You cant add an un-verified {request.GetType()} to {GetType()}");

            _requesting.Add(request);

            // we need to keep all pairs (un-normalised), but we only need one of each.

            var normalised = new AssetPairNormalised(request.Pair);

            if (!_pairs.Any(x=>x.OriginalPair.Equals(normalised.OriginalPair)))
                _pairs.Add(normalised);
        }

        private void Update()
        {
            if (_isDisposed)
                return;

            var hasresult = false;

            foreach (var pair in _pairs.Select(x=>x.OriginalPair).Distinct())
            {
                if (_isDisposed)
                    return;

                var r = ApiCoordinator.GetLatestPrice(_provider, new PublicPriceContext(pair));
                if (r.IsNull)
                {
                    IsFailing = true;
                    continue;
                }

                if (_isDisposed)
                    return;

                hasresult = true;
                AddResult(pair, r.Response);
            }

            if (hasresult && !_isDisposed)
                _messenger.Send(new ExchangeRatesUpdatedMessage());
        }

        private void AddResult(AssetPair pair, LatestPrice response)
        {
            var pairs = _pairs.Where(x => x.OriginalPair.Equals(pair));

            foreach (var npair in pairs)
                _messenger.Send(new ExchangeRateCollected(_provider, npair, response));
        }

        private bool _isDisposed;
        public void Dispose()
        {
            _isDisposed = true;
               _timer?.Dispose();
        }
    }
}