using System;

namespace Prime.Common
{
    public class BalanceResult : IEquatable<BalanceResult>
    {
        public readonly Asset Asset;

        public Money Balance { get; set; }
        public Money Available { get; set; }
        public Money Reserved { get; set; }

        public BalanceResult(Asset asset)
        {
            Asset = asset;
        }

        public bool Equals(BalanceResult other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Asset.Equals(other.Asset);
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
            return Asset.GetHashCode();
        }

        public override string ToString()
        {
            return "A: " + Available + " B: " + Balance + " R: " + Reserved;
        }
    }
}