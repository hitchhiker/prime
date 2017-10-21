using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common.Exchange
{
    public class BidAskData : IEquatable<BidAskData>
    {
        public BidAskData()
        {
            Time = DateTime.UtcNow;
        }

        public BidAskData(Money price, decimal volume)
        {
            Price = price;
            Volume = volume;
        }

        public BidAskData(Money price, decimal volume, DateTime time): this(price, volume)
        {
            Time = time;
        }

        public Money Price { get; set; }
        public decimal Volume { get; set; }
        public DateTime Time { get; set; }

        public bool Equals(BidAskData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Price.Equals(other.Price) && Volume == other.Volume && Time.Equals(other.Time);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BidAskData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Price.GetHashCode();
                hashCode = (hashCode * 397) ^ Volume.GetHashCode();
                hashCode = (hashCode * 397) ^ Time.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"Price: {Price.Display} Volume: {Volume} Time: {Time}";
        }
    }
}
