using System;

namespace Prime.Common
{
    public class AssetPairDiscoveryRequestMessage : IEquatable<AssetPairDiscoveryRequestMessage>
    {
        public AssetPairDiscoveryRequestMessage() { }

        public AssetPairDiscoveryRequestMessage(AssetPairDiscoveryRequestMessage context)
        {
            Pair = context.Pair;
            PeggedEnabled = context.PeggedEnabled;
            ConversionEnabled = context.ConversionEnabled;
        }

        public Network Network { get; set; }
        
        public AssetPair Pair { get; set; }

        public bool PeggedEnabled { get; set; }

        public bool ConversionEnabled { get; set; } = true;

        public bool ReversalEnabled { get; set; } = true;

        public bool Equals(AssetPairDiscoveryRequestMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Network, other.Network) && Equals(Pair, other.Pair) && PeggedEnabled == other.PeggedEnabled && ConversionEnabled == other.ConversionEnabled && ReversalEnabled == other.ReversalEnabled;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssetPairDiscoveryRequestMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Network != null ? Network.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Pair != null ? Pair.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PeggedEnabled.GetHashCode();
                hashCode = (hashCode * 397) ^ ConversionEnabled.GetHashCode();
                hashCode = (hashCode * 397) ^ ReversalEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}