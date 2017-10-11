using System;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeConversionRequest
    {
        public readonly AssetPair Pair;
        public readonly Action<LatestPriceResult> Action;
        public readonly Network Network;

        public ExchangeConversionRequest(AssetPair pair, Action<LatestPriceResult> action, Network network = null)
        {
            Pair = pair;
            Action = action;
            Network = network;
        }

        public bool Match(ExchangeConversionRequest request)
        {
            return Pair.Equals(request.Pair) && Action == request.Action && (Network == null && request.Network == null || Network.Equals(request.Network));
        }

        public void Register()
        {
            
        }

        public void Unregister()
        {
            
        }
    }
}