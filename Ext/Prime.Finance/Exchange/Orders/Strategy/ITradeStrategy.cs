using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Finance.Exchange.Trading_temp
{
    public interface ITradeStrategy : IUniqueIdentifier<ObjectId>
    {
        TradeStrategyContext Context { get; }

        void Start();

        void Cancel();

        TradeStrategyStatus GetStatus();
    }
}