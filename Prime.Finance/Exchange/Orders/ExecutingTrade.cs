using Prime.Finance.Exchange.Trading_temp;

namespace Prime.Finance
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