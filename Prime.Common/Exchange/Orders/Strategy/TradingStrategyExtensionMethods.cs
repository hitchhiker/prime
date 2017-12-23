namespace Prime.Common.Exchange.Trading_temp
{
    public static class TradingStrategyExtensionMethods
    {
        public static bool IsEnded(this ITradeStrategy strategy)
        {
            if (strategy == null)
                return true;

            var status = strategy.GetStatus();
            return status == TradeStrategyStatus.Cancelled || status == TradeStrategyStatus.Failed || status == TradeStrategyStatus.Completed;
        }
    }
}