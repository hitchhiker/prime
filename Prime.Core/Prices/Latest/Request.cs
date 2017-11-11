using System;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Core.Prices.Latest.Messages;
using Prime.Utility.Misc;

namespace Prime.Core.Prices.Latest
{
    internal sealed partial class Request : IEquatable<Request>, IDisposable
    {
        public Request(AssetPair pair, Network network = null)
        {
            NetworkSuggested = network;
            Pair = pair;
        }

        public readonly AssetPair Pair;

        public Network NetworkSuggested { get; private set; }

        public bool IsDiscovered { get; set; }

        public AssetPairNetworks Discovered { get; set; }

        public RequestMessenger Messenger { get; set; }

        public bool IsConvertedPart1 { get; set; }

        public bool IsConvertedPart2 { get; set; }

        public bool IsConverted => IsConvertedPart1 || IsConvertedPart2;

        public bool IsReversed => !IsConverted && IsDiscovered && Pair.Equals(Discovered.Pair.Reversed);

        public LatestPriceResultMessage LastConvert1 { get; set; }

        public LatestPriceResultMessage LastConvert2 { get; set; }

        public MarketPrice LastPrice { get; set; }

        internal DiscoveryRequestProcessor Processor { get; set; }

        public void Dispose()
        {
            Messenger?.Dispose();
        }
    }

    internal sealed partial class Request
    {
        public bool Equals(AssetPair pair, bool isConvertedPart1, bool isConvertedPart2, Network networkSuggested)
        {
            return Equals(Pair, pair) && IsConvertedPart1 == isConvertedPart1 && IsConvertedPart2 == isConvertedPart2 && NetworkSuggested.EqualOrBothNull(networkSuggested);
        }

        public bool Equals(Request other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Pair, other.IsConvertedPart1, other.IsConvertedPart2, other.NetworkSuggested);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Request)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Pair != null ? Pair.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsConvertedPart1.GetHashCode();
                hashCode = (hashCode * 397) ^ IsConvertedPart2.GetHashCode();
                hashCode = (hashCode * 397) ^ (NetworkSuggested != null ? NetworkSuggested.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}