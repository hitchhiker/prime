namespace Prime.Common.Exchange.Trading_temp
{
    public class TradeStrategyContext
    {
        public readonly UserContext UserContext;
        public readonly Network Network;
        public readonly AssetPair Pair;
        public readonly bool IsBuy;
        public readonly decimal Quantity;

        public TradeStrategyContext(UserContext uCtx, Network network, bool isBuy, AssetPair pair, decimal quantity)
        {
            UserContext = uCtx;
            Network = network;
            IsBuy = isBuy;
            Pair = pair;
            Quantity = quantity;
        }

        public bool IsTesting { get; set; }
    }
}