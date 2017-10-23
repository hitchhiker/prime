using System;

namespace Prime.Common.Exchange.Rates
{
    public class FxRequestMessage
    {
        public readonly AssetPair Pair;
        public readonly Action<LatestPriceResultMessage> Action;
        public readonly Network Network;

        public FxRequestMessage(AssetPair pair, Action<LatestPriceResultMessage> action, Network network = null)
        {
            Pair = pair;
            Action = action;
            Network = network;
        }

        public bool Match(FxRequestMessage requestMessage)
        {
            return Pair.Equals(requestMessage.Pair) && Action == requestMessage.Action && (Network == null && requestMessage.Network == null || Network.Equals(requestMessage.Network));
        }

        public void Register()
        {
            
        }

        public void Unregister()
        {
            
        }
    }
}