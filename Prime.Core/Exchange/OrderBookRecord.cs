using System;
using Prime.Core.Exchange;

namespace Prime.Core
{
    public class OrderBookRecord : IEquatable<OrderBookRecord>
    {
        public OrderBookRecord()
        {
            
        }

        public OrderBookRecord(BidAskData data)
        {
            Data = data;
        }

        public BidAskData Data { get; set; }
        public OrderBookType Type { get; set; }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(OrderBookType), Type)}: {Data.Price.Display}";
        }

        public bool Equals(OrderBookRecord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Data, other.Data) && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OrderBookRecord) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Data != null ? Data.GetHashCode() : 0) * 397) ^ (int) Type;
            }
        }
    }
}
