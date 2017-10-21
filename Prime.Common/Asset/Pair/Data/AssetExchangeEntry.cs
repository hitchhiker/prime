using System;

namespace Prime.Common
{
    public class AssetExchangeEntry : IEquatable<AssetExchangeEntry>
    {
        private AssetExchangeEntry() { }

        public AssetExchangeEntry(Network network)
        {
            Network = network;
        }

        [Bson]
        public Network Network { get; private set; }
        [Bson]
        public string Type { get; set; }
        [Bson]
        public string Flags { get; set; }
        [Bson]
        public double Price { get; set; }
        [Bson]
        public DateTime UtcLastUpdate { get; set; }
        [Bson]
        public double LastVolume { get; set; }
        [Bson]
        public double LastVolumeTo { get; set; }
        [Bson]
        public double Volume24Hour { get; set; }
        [Bson]
        public double Volume24HourTo { get; set; }
        [Bson]
        public double Open24Hour { get; set; }
        [Bson]
        public double High24Hour { get; set; }
        [Bson]
        public double Low24Hour { get; set; }

        public bool Equals(AssetExchangeEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Network, other.Network);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssetExchangeEntry) obj);
        }

        public override int GetHashCode()
        {
            return (Network != null ? Network.GetHashCode() : 0);
        }
    }
}