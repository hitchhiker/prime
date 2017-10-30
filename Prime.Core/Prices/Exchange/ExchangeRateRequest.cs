using System;
using Prime.Utility.Misc;

namespace Prime.Common.Exchange.Rates
{
    public class ExchangeRateRequest : IEquatable<ExchangeRateRequest>
    {
        public readonly AssetPair Pair;
        public readonly Network Network;
        
        public ExchangeRateRequest(AssetPair pair, Network network = null)
        {
            Pair = pair;
            Network = network;
        }

        public bool Equals(ExchangeRateRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Pair, other.Pair) && Network.EqualOrBothNull(other.Network);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExchangeRateRequest) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Pair != null ? Pair.GetHashCode() : 0) * 397) ^ (Network != null ? Network.GetHashCode() : 0);
            }
        }
    }
}