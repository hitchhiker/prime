using LiteDB;

namespace Prime.Core.Exchange.Trading_temp.Messages
{
    public class TradeMessage
    {
        public readonly ObjectId TradeId;
        public TradeMessage(ObjectId tradeId)
        {
            TradeId = tradeId;
        }
    }
}