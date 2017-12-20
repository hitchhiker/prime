using System;
using Prime.Common.Exchange;

namespace Prime.Common
{
    public class OrderBookRecord
    {
        /// <summary>
        /// TODO: Just a note to Alex (delete when read)
        /// Moved the type to the constructor, made the set properties private, made the empty constructor private (for deserialisation only)
        /// Also only use 'IEquatable' if you have a specific need, as it was here it served no purpose (that I could see)
        /// Removed TIMESTAMP -> unreliable zones etc, it's safer if we use our known UTC NOW time, as we can be sure that's accurate.
        /// </summary>

        private OrderBookRecord() : base() { }

        private OrderBookRecord(OrderType type, Money price, Money volume)
        {
            Type = type;
            Price = price;
            Volume = volume;
            UtcUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Do not use this constructor
        /// </summary>
        internal static OrderBookRecord CreateInternal(OrderType type, Money price, Money volume)
        {
            return new OrderBookRecord(type, price, volume);
        }

        public readonly OrderType Type;
        public readonly Money Price;
        public readonly Money Volume;
        public readonly DateTime UtcUpdated;

        public override string ToString()
        {
            return Type + " " + Price.ToString() + " [" + Volume.ToString() + "]";
        }

        public OrderBookRecord AsQuote(Asset asset)
        {
            if (Price.Asset.Id == asset.Id)
                return this;

            return new OrderBookRecord(Type, Price.ReverseAsset(asset), new Money(Volume * Price, asset));
        }
    }
}
