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
        private readonly IPublicPricesProvider _providerS;
        private readonly bool _multiPrice;
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
            _providerS = context.Provider as IPublicPricesProvider;
            _multiPrice = _providerS != null;
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
                    Update();
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
                _pairRequests.Add(requestMessage.PairForProvider);
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

            var hasresult = _multiPrice ? RequestMultipleEntries() : RequestSingleEntries();

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

                var r = ApiCoordinator.GetPrice(_provider, new PublicPriceContext(pair));
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

            foreach (var lp in r.Response)
            {
                if (_isDisposed)
                    return false;

                hasresult = true;
                SendResults(lp);
            }
            return hasresult;
        }

        private void SendResults(LatestPrice response)
        {
            var requests = _verifiedRequests.Where(x => x.PairForProvider.Equals(response.Pair));

            foreach (var request in requests)
                SendResults(response, request);
        }

        private void SendResults(LatestPrice price, LatestPriceRequest request)
        {
            var priceNormalised = request.Providers.IsPairReversed ? price.Reverse() : price;

            var resultMsg = request.LastResult = new LatestPriceResultMessage(_provider, request.PairForProvider, priceNormalised);
            request.LastPrice = priceNormalised;

            if (Equals(priceNormalised.QuoteAsset, "BTC".ToAssetRaw()) && Equals(priceNormalised.Price.Asset, "USD".ToAssetRaw()) && priceNormalised.Price<1000)
                throw new Exception("WTF");

            _messenger.SendAsync(resultMsg);
            
            if (request.IsConverted)
                SendConverted(request);
        }

        private void SendConverted(LatestPriceRequest request)
        {
            if (request.ConvertedOther?.LastResult == null || request.ConvertedOther?.LastPrice == null)
                return;

            var p1 = request.IsConvertedPart1 ? request : request.ConvertedOther;
            var p2 = request.IsConvertedPart2 ? request : request.ConvertedOther;

            var resultMsg = request.LastResult = new LatestPriceResultMessage(p1.Pair, p1.LastPrice, p2.LastPrice, p1.Providers.Provider, p2.Providers.Provider);
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