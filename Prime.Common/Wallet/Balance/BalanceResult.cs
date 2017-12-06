using System;

namespace Prime.Common
{
    public class BalanceResult : IEquatable<BalanceResult>
    {
        public Asset Asset => !Balance.Asset.IsNone() ? Balance.Asset : (!Available.Asset.IsNone() ? Available.Asset : Reserved.Asset);

        [Bson]
        public Money Balance { get; set; }

        [Bson]
        public Money Available { get; set; }

        [Bson]
        public Money Reserved { get; set; }

        [Bson]
        public AssetPosition AssetPosition { get; private set; }

        public Network Network { get; }

        public BalanceResult(INetworkProvider provider) : this(provider.Network) { }

        public BalanceResult(Network network, Money? availableBalance = null)
        {
            Network = network;
            if (availableBalance != null)
            {
                Available = availableBalance.Value;
                AssetPosition = new AssetPosition(network, Available.Asset);
            }
            else
                AssetPosition = new AssetPosition(network, Asset.None);
        }

        public override string ToString()
        {
            return "A: " + Available + " B: " + Balance + " R: " + Reserved;
        }

        public bool Equals(BalanceResult other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Asset, other.Asset) && Equals(Network, other.Network);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BalanceResult) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Asset != null ? Asset.GetHashCode() : 0) * 397) ^ (Network != null ? Network.GetHashCode() : 0);
            }
        }
    }
}