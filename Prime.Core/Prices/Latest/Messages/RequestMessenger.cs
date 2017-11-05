using System;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Utility;

namespace Prime.Core.Prices.Latest
{
    /// <summary>
    /// Handles conversion/reversed messages by matching / modifying and combining results and emiting a converted message result.
    /// </summary>
    internal sealed class RequestMessenger : IDisposable
    {
        private readonly Request _request;
        private readonly IMessenger _m = DefaultMessenger.I.Default;
        private readonly AssetPairNetworks _nets;
        private readonly bool _isReversed;
        private readonly bool _isConverted;
        private readonly object _lock = new object();

        public RequestMessenger(Request request)
        {
            _request = request;
            _nets = _request.Discovered;
            _isReversed = _request.IsReversed;
            _isConverted = _request.IsConverted;

            if (_nets==null || !_request.IsDiscovered || !(_isReversed || _isConverted))
                return;

            if (_isConverted)
                _m.RegisterAsync<LatestPriceResultMessage>(this, DoConversion);
            else if (_isReversed)
                _m.RegisterAsync<LatestPriceResultMessage>(this, DoReversal);
        }

        private void DoReversal(LatestPriceResultMessage m)
        {
            lock (_lock)
            {
                if (_nets.Pair.Equals(m.Pair))
                    _m.SendAsync(new LatestPriceResultMessage(m.Provider, m.MarketPrice.Reverse()));
            }
        }

        private void DoConversion(LatestPriceResultMessage m)
        {
            if (m.IsConverted)
                return;

            lock (_lock)
            {
                var converted = TryConversion(_nets.ConversionPart1, m) || TryConversion(_nets.ConversionPart2, m);
                if (!converted)
                    return;

                EmitConversion();
            }
        }

        private bool TryConversion(AssetPairNetworks networks, LatestPriceResultMessage m)
        {
            if (TryConversion(networks, m, false))
                return true;
            if (TryConversion(networks, m, true))
                return true;
            return false;
        }

        private bool TryConversion(AssetPairNetworks networks, LatestPriceResultMessage m, bool reverse)
        {
            var pair = reverse ? networks.Pair.Reverse() : networks.Pair;
            if (!pair.Equals(m.Pair))
                return false;

            var message = reverse ? new LatestPriceResultMessage(m.Provider, m.MarketPrice.Reverse()) : m;
            if (networks.IsConversionPart1)
                _request.LastConvert1 = message;
            else
                _request.LastConvert2 = message;

            return true;
        }

        private void EmitConversion()
        {
            if (_request.LastConvert1 == null || _request.LastConvert2 == null)
                return;

            var p1 = _request.LastConvert1;
            var p2 = _request.LastConvert2;

            var reverse1 = Equals(p1.Pair.Asset1, _request.Pair.Asset1);
            var reverse2 = Equals(p2.Pair.Asset1, _request.Pair.Asset1);

            var p1F = reverse1 ? 1 / p1.Price : p1.Price;
            var p2F = reverse2 ? 1 / p2.Price : p2.Price;

            var price = new Money(p1F * p2F, _request.Pair.Asset2);
            var market = new MarketPrice(_request.Pair.Asset1, price);

            var message = new LatestPriceResultMessage(market, _nets.Intermediary, p1.MarketPrice, p2.MarketPrice, p1.Provider, p2.Provider);

            _m.SendAsync(message);
        }

        public void Dispose()
        {
            _m.UnregisterAsync(this);
        }
    }
}