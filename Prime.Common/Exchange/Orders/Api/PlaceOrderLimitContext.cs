using System;
using Prime.Utility;

namespace Prime.Common
{
    public class PlaceOrderLimitContext : NetworkProviderPrivateContext
    {
        /// <summary>
        /// Places buy/sell limit order to specified market. Does not handle reversed exchanges, this is left to upper-providers.
        /// </summary>
        /// <param name="userContext">User specified information.</param>
        /// <param name="pair">The market where limit order is to be placed.</param>
        /// <param name="isBuy">Type of limit order - sell or buy.</param>
        /// <param name="quantity">If exchange is not reversed, the quantity of base asset (e.g. for BTC-USD market the quantity is expressed in BTC).</param>
        /// <param name="rate">If exchange is not reversed, the rate in quote asset currency (e.g. for BTC-USD market the rate is expressed in USD).</param>
        /// <param name="logger"></param>
        public PlaceOrderLimitContext(UserContext userContext, AssetPair pair, bool isBuy, Money quantity, Money rate, ILogger logger = null) : base(userContext, logger)
        {
            if (Equals(quantity.Asset, Asset.None))
                throw new ArgumentException($"Asset should be specified for {nameof(quantity)} parameter", nameof(quantity));

            Pair = pair;
            IsBuy = isBuy;
            Quantity = quantity;
            Rate = rate;

            if (!pair.Has(rate.Asset))
                throw new ArgumentException($"The {nameof(rate)}'s asset does not belong to this market '{pair}'");
            if (!pair.Has(quantity.Asset))
                throw new ArgumentException($"The {nameof(quantity)}'s asset does not belong to this market '{pair}'");
        }

        public AssetPair Pair { get; }
        public bool IsBuy { get; }
        public Money Quantity { get; }
        public Money Rate { get; }

        public bool IsSell => !IsBuy;
    }
}