using System;

namespace Prime.Common
{
    public class BalanceResult : IEquatable<BalanceResult>
    {
        public Asset Asset => !Balance.Asset.IsNone() ? Balance.Asset : (!Available.Asset.IsNone() ? Available.Asset : Reserved.Asset);

        public Money Balance { get; set; }
        public Money Available { get; set; }
        public Money Reserved { get; set; }

        public Network Network { get; private set; }

        public BalanceResult(INetworkProvider provider) : this(provider.Network) { }

        public BalanceResult(Network network)
        {
            Network = network;
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