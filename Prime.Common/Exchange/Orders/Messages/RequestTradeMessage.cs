using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common.Exchange.Trading_temp.Messages
{
    public class RequestTradeMessage : TradeMessage
    {
        public readonly ITradeStrategy Strategy;

        public RequestTradeMessage(ITradeStrategy strategy) : base(strategy.Id)
        {
            Strategy = strategy;
        }
    }
}
