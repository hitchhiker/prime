using System;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Utility;

namespace Prime.Core.Prices.Latest
{
    /// <summary>
    /// Handles conversion/reversed messages by modifying / combining results to form the converted message result.
    /// </summary>
    internal sealed class RequestMessenger : IDisposable
    {
        private readonly Request _request;
        private readonly IMessenger _m = DefaultMessenger.I.Default;
        private readonly AssetPairNetworks _nets;
        private readonly bool _isReversed;
        private readonly bool _isConverted;

        public RequestMessenger(Request request)
        {
            _request = request;
            _nets = _request.Discovered;
            _isReversed = _request.IsReversed;
            _isConverted = _request.IsConverted;

            if (_nets==null || !_request.IsDiscovered || !_isReversed || !_isConverted)
                return;

            if (_isConverted)
                _m.RegisterAsync<LatestPriceResultMessage>(this, DoConversion);
            else if (_isReversed)
                _m.RegisterAsync<LatestPriceResultMessage>(this, DoReversal);
        }

        private void DoReversal(LatestPriceResultMessage m)
        {
            if (_nets.Pair.Equals(m.Pair))
            {
                var lp = new LatestPrice(m.Price.ReverseAsset(m.Pair.Asset2), m.Pair.Asset1);
                _m.SendAsync(new LatestPriceResultMessage(m.Provider, lp));
            }
        }

        private void DoConversion(LatestPriceResultMessage m)
        {
            /*if (_nets.Pair.Equals(m.Pair))
            {
                var lp = new LatestPrice(m.Price.ReverseAsset(m.Pair.Asset2), m.Pair.Asset1);
                _m.SendAsync(new LatestPriceResultMessage(m.Provider, lp));
            }*/
        }

        public void Dispose()
        {
            _m.UnregisterAsync(this);
        }
    }
}