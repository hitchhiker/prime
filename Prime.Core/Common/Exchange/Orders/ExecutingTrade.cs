using Prime.Core.Exchange.Trading_temp;

namespace Prime.Core
{
    public class ExecutingTrade
    {
        public readonly ITradeStrategy Strategy;

        public ExecutingTrade(ITradeStrategy strategy)
        {
            Strategy = strategy;
        }
    }
}