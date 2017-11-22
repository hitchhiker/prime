using Prime.Common.Exchange;

namespace Prime.Common
{
    public class OrderBookRecord : BidAskData
    {
        /// <summary>
        /// TODO: Just a note to Alex (delete when read)
        /// Moved the type to the constructor, made the set properties private, made the empty constructor private (for deserialisation only)
        /// Also only use 'IEquatable' if you have a specific need, as it was here it served no purpose (that I could see)
        /// Removed TIMESTAMP -> unreliable zones etc, it's safer if we use our known UTC NOW time, as we can be sure that's accurate.
        /// </summary>

        private OrderBookRecord() : base() { }

        public OrderBookRecord(OrderType type, Money price, decimal volume) : base(price, volume)
        {
            Type = type;
        }

        public OrderType Type { get; private set; }

        public override string ToString()
        {
            return Type + " " + Price.ToString() + " [" + Volume + "]";
        }
    }
}
