using System;
using Prime.Utility;

namespace Prime.Common
{
    public class PlaceOrderLimitContext : NetworkProviderPrivateContext
    {
        /// <summary>
        /// Places buy/sell limit order to specified market.
        /// </summary>
        /// <param name="userContext">User specified information.</param>
        /// <param name="pair">The market where limit order is to be placed.</param>
        /// <param name="isBuy">Type of limit order - sell or buy.</param>
        /// <param name="quantity">The quantity of base asset (e.g. for BTC-USD market quantity of BTC).</param>
        /// <param name="rate">The rate in quote asset currency (e.g. for BTC-USD market rate expressed in USD).</param>
        /// <param name="logger"></param>
        public PlaceOrderLimitContext(UserContext userContext, AssetPair pair, bool isBuy, decimal quantity, Money rate, ILogger logger = null) : base(userContext, logger)
        {
            Pair = pair;
            IsBuy = isBuy;
            Quantity = quantity;
            Rate = rate;

            if (!pair.Has(rate.Asset))
                throw new ArgumentException($"The {nameof(rate)}'s asset does not belong to this market '{pair}'");

            if(!pair.Asset2.Equals(rate.Asset))
                throw new ArgumentException($"Wrong currency rate asset is set for '{pair}' market - must be {pair.Asset2}");
        }

        public AssetPair Pair { get; }
        public bool IsBuy { get; }
        public decimal Quantity { get; }
        public Money Rate { get; }

        public bool IsSell => !IsBuy;
    }
}