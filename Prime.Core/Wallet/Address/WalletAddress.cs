using System;
using Prime.Utility;

namespace Prime.Core
{
    public class WalletAddress : IEquatable<WalletAddress>
    {
        private WalletAddress() { }

        public WalletAddress(IWalletService service, Asset asset)
        {
            Network = service.Network;
            UtcCreated = UtcLastChecked = DateTime.UtcNow;
            Asset = asset;
        }

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public Asset Asset { get; private set; }

        [Bson]
        public string Address { get; set; }

        [Bson]
        public string Tag { get; set; }

        [Bson]
        public DateTime UtcCreated { get; set; }

        [Bson]
        public DateTime UtcLastChecked { get; set; }

        [Bson]
        public DateTime ExpiresUtc { get; set; }

        public string ServiceName => Network?.Name ?? "Unknown";

        public override string ToString()
        {
            return ServiceName + " " + Asset + " " + Address + " " + Tag;
        }

        public bool IsFresh()
        {
            return UtcLastChecked.IsWithinTheLast(TimeSpan.FromDays(7));
        }

        public bool IsExpired()
        {
            return ExpiresUtc != DateTime.MinValue && DateTime.UtcNow >= ExpiresUtc;
        }

        public bool Equals(WalletAddress other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Network, other.Network) && Equals(Asset, other.Asset) && string.Equals(Address, other.Address) && string.Equals(Tag, other.Tag);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WalletAddress) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Network != null ? Network.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Asset != null ? Asset.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Tag != null ? Tag.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}