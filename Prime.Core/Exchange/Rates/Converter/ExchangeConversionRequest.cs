using System;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeConversionRequest
    {
        public readonly AssetPair Pair;
        public readonly object Subscriber;
        public readonly Action<LatestPriceResult> Action;
        public readonly Network Network;

        public ExchangeConversionRequest(object subscriber, AssetPair pair, Action<LatestPriceResult> action, Network network = null)
        {
            Pair = pair;
            Action = action;
            Network = network;
            Subscriber = subscriber;
        }

        public bool Match(ExchangeConversionRequest request)
        {
            return Subscriber == request.Subscriber && Pair.Equals(request.Pair) && Action == request.Action &&
                   (Network == null && request.Network == null || Network.Equals(request.Network));
        }

        public void Register()
        {
            
        }
    }
}