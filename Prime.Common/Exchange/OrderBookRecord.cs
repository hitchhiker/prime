using System;
using Prime.Common.Exchange;

namespace Prime.Common
{
    public class OrderBookRecord
    {
        private OrderBookRecord() : base() { }

        private OrderBookRecord(OrderType type, Money price, Money volume)
        {
            if (price.Asset.IsNone())
                throw new ArgumentException($"{nameof(price)} {nameof(Asset)} cannot be 'None'");

            if (volume.Asset.IsNone())
                throw new ArgumentException($"{nameof(volume)} {nameof(Asset)} cannot be 'None'");

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
        public DateTime UtcUpdated { get; private set; }

        public override string ToString()
        {
            return $"{Type} {Price.ToDecimalValue()} [{Volume.ToDecimalValue()}]";
        }

        public OrderBookRecord Reverse(Asset asset)
        {
            if (Price.Asset.Id == asset.Id)
                throw new Exception("Cant reverse to the same 'Asset'");

            return new OrderBookRecord(Type == OrderType.Ask ? OrderType.Bid : OrderType.Ask, Price.ReverseAsset(asset), new Money(Volume * Price, asset));
        }

        public OrderBookRecord Clone()
        {
            return new OrderBookRecord(Type, Price, Volume) {UtcUpdated = UtcUpdated};
        }
    }
}
