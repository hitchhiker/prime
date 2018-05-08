using LiteDB;
using Prime.Base;

namespace Prime.Finance.Exchange.Trading_temp.Messages
{
    public class TradeStatusChangedMessage  : TradeMessage {

        public TradeStrategyStatus Status { get; }

        public TradeStatusChangedMessage(ObjectId tradeId, TradeStrategyStatus status) : base(tradeId)
        {
            Status = status;
        }
    }
}