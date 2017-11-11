using System;
using System.Collections.Generic;
using System.Timers;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;
using System.Linq;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Core.Prices.Latest;

namespace Prime.Core
{
    internal sealed class LatestPriceProvider : IDisposable
    {
        private readonly LatestPriceProviderContext _context;
        public readonly IPublicPriceSuper Provider;
        private readonly IPublicPriceProvider _providerP;
        private readonly IPublicPricesProvider _providerS;
        private readonly IPublicAssetPricesProvider _providerA;
        private readonly bool _hasPrices;
        private readonly bool _hasPrice;
        private readonly bool _hasPriceA;
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
            _providerP = context.Provider as IPublicPriceProvider;
            _providerS = context.Provider as IPublicPricesProvider;
            _providerA = context.Provider as IPublicAssetPricesProvider;

            if (context.Provider == null)
                throw new ArgumentException($"{nameof(context.Provider)} is null in {GetType()}");

            _hasPrices = _providerS != null;
            _hasPriceA = _providerA != null;
            _hasPrice = _providerP != null || _providerA != null;

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

            var hasresult = ((_hasPrices && _pairRequests.Count>1) || !_hasPrice) ? RequestMultipleEntries() : RequestSingleEntries();

            if (hasresult && !_isDisposed)
                _messenger.Send(new LatestPricesUpdatedMessage());
        }

        private bool RequestSingleEntries()
        {
            var hasresult = false;

            foreach (var pair in _pairRequests)
            {
                if (_isDisposed)
                    return false;

                var r = _hasPriceA ? ApiCoordinator.GetPrice(_providerA, new PublicPriceContext(pair)) : ApiCoordinator.GetPrice(_providerP, new PublicPriceContext(pair));

                if (r.IsNull)
                {
                    IsFailing = true;
                    continue;
                }

                if (_isDisposed)
                    return false;

                hasresult = true;
                SendResults(r.Response);
            }
            return hasresult;
        }
        
        private bool RequestMultipleEntries()
        {
            var hasresult = false;

            if (_isDisposed)
                return false;

            var r = ApiCoordinator.GetPrices(_providerS, new PublicPricesContext(_pairRequests));
            if (r.IsNull)
            {
                IsFailing = true;
                return false;
            }

            if (_isDisposed)
                return false;

            foreach (var lp in r.Response.MarketPrices)
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