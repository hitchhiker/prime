using System;
using Prime.Common;

namespace Prime.Common
{
    public class AssetPosition : IEquatable<AssetPosition>
    {
        private AssetPosition(){}

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public Asset Asset { get; private set; }

        public AssetPosition(Network network, Asset asset)
        {
            Network = network;
            Asset = asset;
        }

        public bool Equals(AssetPosition other)
        {
            return Equals(Network?.Id, other.Network?.Id) && Equals(Asset?.Id, other.Asset?.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AssetPosition && Equals((AssetPosition) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Network != null ? Network.Id.GetHashCode() : 0) * 397) ^ (Asset != null ? Asset.Id.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return Network?.Name + " " + Asset?.ShortCode;
        }
    }
}