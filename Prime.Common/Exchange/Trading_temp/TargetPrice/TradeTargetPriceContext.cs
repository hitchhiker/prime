using System;

namespace Prime.Common.Exchange.Trading_temp
{
    public class TradeTargetPriceContext : TradeStrategyContext
    {
        public readonly Money Price;
        public readonly decimal PercentageTolerance;

        public TradeTargetPriceContext(UserContext uCtx, Network network, bool isBuy, AssetPair pair, Money price, decimal quantity, decimal? percentageTolerance = null) : base(uCtx, network, isBuy, pair, quantity)
        {
            if (price.Asset.Id != pair.Asset2.Id)
                throw new ArgumentException($"{nameof(price)} asset does not match the {nameof(AssetPair)}'s quote {nameof(Asset)}");

            Price = price;
            PercentageTolerance = percentageTolerance ?? 0;
        }
    }
}