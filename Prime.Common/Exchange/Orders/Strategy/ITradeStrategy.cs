using LiteDB;
using Prime.Common;

namespace Prime.Common.Exchange.Trading_temp
{
    public interface ITradeStrategy : IUniqueIdentifier<ObjectId>
    {
        TradeStrategyContext Context { get; }

        void Start();

        void Cancel();

        TradeStrategyStatus GetStatus();
    }
}