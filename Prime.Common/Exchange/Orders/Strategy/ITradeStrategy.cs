using LiteDB;
using Prime.Utility;

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