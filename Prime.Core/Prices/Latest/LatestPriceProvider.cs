using System;
using System.Collections.Generic;
using System.Timers;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;
using System.Linq;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Core.Prices.Latest;

namespace Prime.Core
{
    internal sealed class LatestPriceProvider : IDisposable
    {
        private readonly LatestPriceProviderContext _context;
        public readonly IPublicPricingProvider Provider;
        //private readonly UniqueList<Request> _verifiedRequests = new UniqueList<Request>();
        private readonly UniqueList<AssetPair> _pairRequests = new UniqueList<AssetPair>();
        private readonly IMessenger _messenger;
        private readonly Timer _timer;
        public readonly Network Network;
        private readonly object _timerLock = new object();

        public LatestPriceProvider(LatestPriceProviderContext context)
        {
            _context = context;
            Provider = context.Provider;

            if (context.Provider == null)
                throw new ArgumentException($"{nameof(context.Provider)} is null in {GetType()}");

            Network = Provider.Network;
            _messenger = context.Aggregator.M;
            _timer = new Timer(10) {AutoReset = false};
            _timer.Elapsed += delegate { UpdateTick(); };
            _timer.Start();
        }

        private DateTime _utcLastUpdate = DateTime.MinValue;

        private void UpdateTick()
        {
            lock (_timerLock)
            {
                if (_pairRequests.Count != 0 && !_utcLastUpdate.IsWithinTheLast(_context.PollingSpan))
                {
                    Run();
                    _utcLastUpdate = DateTime.UtcNow;
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
                Run();
            }
        }

        public bool IsFailing { get; private set; }

        internal void SyncPairRequests(IEnumerable<AssetPair> pairs)
        {
            lock (_timerLock)
            {
                _pairRequests.Clear();
                _pairRequests.AddRange(pairs);
            }
        }

        public bool HasRequests()
        {
            lock (_timerLock)
            {
                return _pairRequests.Any();
            }
        }

        private void Run()
        {
            if (_isDisposed)
                return;

            var hasresult = RequestApi();

            if (hasresult && !_isDisposed)
                _messenger.Send(new LatestPricesUpdatedMessage());
        }

        private bool RequestApi()
        { 
            var hasresult = false;

            if (_isDisposed)
                return false;

            var r = AsyncContext.Run(()=> PricingProvider.I.GetAsync(Provider.Network, _pairRequests, new PricingProviderContext(){UseDirect = true}));
            if (r==null)
            {
                IsFailing = true;
                return false;
            }

            if (_isDisposed)
                return false;

            foreach (var lp in r)
            {
                if (_isDisposed)
                    return false;

                hasresult = true;
                SendResults(lp);
            }
            return hasresult;
        }

        private void SendResults(MarketPrice response)
        {
            var resultMsg = new LatestPriceResultMessage(Provider, response);
            _messenger.SendAsync(resultMsg);
        }

        private bool _isDisposed;
        public void Dispose()
        {
            _isDisposed = true;
               _timer?.Dispose();
        }
    }
}