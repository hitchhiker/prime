using System;
using Prime.Core;

namespace Prime.Finance
{
    public class AssetPairPosition : IEquatable<AssetPosition>
    {
        private AssetPairPosition() { }

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public AssetPair Pair { get; private set; }

        public AssetPairPosition(Network network, AssetPair pair)
        {
            Network = network;
            Pair = pair;
        }

        public bool Equals(AssetPosition other)
        {
            return Equals(Network?.Id, other.Network?.Id) && Equals(Pair?.Id, other.Asset?.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AssetPosition && Equals((AssetPosition)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Network != null ? Network.Id.GetHashCode() : 0) * 397) ^ (Pair != null ? Pair.Id.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return Network?.Name + " " + Pair?.ToString();
        }
    }
}