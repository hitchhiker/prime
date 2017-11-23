namespace Prime.Common.Exchange.Trading_temp.Messages
{
    public class ResponseTradeMessage : TradeMessage
    {
        public readonly ITradeStrategy Strategy;

        public ResponseTradeMessage(ITradeStrategy strategy) : base(strategy.Id)
        {
            Strategy = strategy;
        }
    }
}