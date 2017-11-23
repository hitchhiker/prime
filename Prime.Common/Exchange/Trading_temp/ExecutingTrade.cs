using Prime.Common.Exchange.Trading_temp;

namespace Prime.Common
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