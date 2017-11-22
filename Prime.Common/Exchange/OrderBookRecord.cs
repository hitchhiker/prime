using System;
using Prime.Common.Exchange;

namespace Prime.Common
{
    public class OrderBookRecord : BidAskData
    {
        /// <summary>
        /// Alex: Moved the type to the constructor, made the set properties private, made the empty constructor private (for deserialisation only)
        /// Also only use 'IEquatable' if you have a specific need, as it was here it served no purpose (that I can see)
        /// </summary>

        public OrderBookRecord(OrderBookType type, Money price, decimal volume, DateTime? utcUpdated = null) : base(price, volume, utcUpdated)
        {
            Type = type;
        }

        public OrderBookType Type { get; private set; }

        public override string ToString()
        {
            return Type + " " + base.ToString();
        }
    }
}
