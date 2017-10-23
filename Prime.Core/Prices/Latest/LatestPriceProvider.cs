using System;
using System.Collections.Generic;
using System.Timers;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;
using System.Linq;
using Prime.Common;
using Prime.Common.Exchange.Rates;

namespace Prime.Core
{
    public class LatestPriceProvider : IDisposable
    {
        private readonly LatestPriceProviderContext _context;
        private readonly IPublicPriceProvider _provider;
        private readonly UniqueList<LatestPriceRequest> _verifiedRequests = new UniqueList<LatestPriceRequest>();
        private readonly UniqueList<AssetPair> _pairRequests = new UniqueList<AssetPair>();
        private readonly IMessenger _messenger;
        private readonly Timer _timer;
        public readonly Network Network;
        private readonly object _timerLock = new object();

        public LatestPriceProvider(LatestPriceProviderContext context)
        {
            _context = context;
            _provider = context.Provider;
            Network = _provider.Network;
            _messenger = context.Aggregator.M;
            _timer = new Timer(10) {AutoReset = false};
            _timer.Elapsed += delegate { UpdateTick(); };
            _timer.Start();
        }

        private DateTime _utcLastUpdate = DateTime.MinValue;
        protected void UpdateTick()
        {
            lock (_timerLock)
            {
                if (_pairRequests.Count != 0 && !_utcLastUpdate.IsWithinTheLast(_context.PollingSpan))
                {
                    _utcLastUpdate = DateTime.UtcNow;
                    
                        Update();
                        IsFailing = true;
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

        public void SyncVerifiedRequests(IEnumerable<LatestPriceRequest> requests)
        {
            lock (_timerLock)
            {
                _verifiedRequests.Clear();
                _pairRequests.Clear();

                foreach (var req in requests)
                    AddVerifiedRequest(req);
            }
        }

        public void AddVerifiedRequest(LatestPriceRequest requestMessage)
        {
            lock (_timerLock)
            {
                if (!requestMessage.IsVerified)
                    throw new ArgumentException($"You cant add an un-verified {requestMessage.GetType()} to {GetType()}");

                _verifiedRequests.Add(requestMessage);
                _pairRequests.Add(requestMessage.PairRequestable);
            }
        }

        public bool HasRequests()
        {
            lock (_timerLock)
            {
                return _pairRequests.Any();
            }
        }

        private void Update()
        {
            if (_isDisposed)
                return;

            var hasresult = false;

            foreach (var pair in _pairRequests)
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
                _messenger.Send(new LatestPricesUpdatedMessage());
        }

        private void AddResult(AssetPair pair, LatestPrice response)
        {
            var requests = _verifiedRequests.Where(x => x.PairRequestable.Equals(pair));

            foreach (var request in requests)
                Collect(response, request);
        }

        private void Collect(LatestPrice response, LatestPriceRequest request)
        {
            var collected = request.LastResult = new LatestPriceResultMessage(_provider, request.IsConverted ? request.PairRequestable : request.Pair, response, request.Providers.IsReversed);

            _messenger.Send(collected);

            if (request.IsConverted)
                SendConverted(request, collected, request.ConvertedOther?.LastResult);
        }

        private void SendConverted(LatestPriceRequest request, LatestPriceResultMessage recent, LatestPriceResultMessage other)
        {
            if (other != null)
                _messenger.Send(new LatestPriceResultMessage(request.Pair, request.IsConvertedPart1, recent, other));
        }

        private bool _isDisposed;
        public void Dispose()
        {
            _isDisposed = true;
               _timer?.Dispose();
        }
    }
}