using LiteDB;
using Prime.Core;

namespace Prime.Core.Exchange.Trading_temp
{
    public interface ITradeStrategy : IUniqueIdentifier<ObjectId>
    {
        TradeStrategyContext Context { get; }

        void Start();

        void Cancel();

        TradeStrategyStatus GetStatus();
    }
}