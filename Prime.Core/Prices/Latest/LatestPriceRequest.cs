using System;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Utility;

namespace Prime.Core
{
    public partial class LatestPriceRequest : IEquatable<LatestPriceRequest>
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        public readonly AssetPair Pair;

        public LatestPriceRequest(AssetPair pair, Network network = null)
        {
            Network = NetworkSuggested = network;
            Pair = pair;
        }

        private volatile AssetPairDiscoveryRequestMessage _discoveryRequestRequest;

        public AssetPair PairRequestable { get; private set; }

        public bool IsVerified { get; private set; }

        public AssetPairProviders Providers { get; private set; }

        public LatestPriceRequest ConvertedOther { get; private set; }

        public bool IsConvertedPart1 { get; private set; }

        public bool IsConvertedPart2 { get; private set; }

        public bool IsConverted => IsConvertedPart1 || IsConvertedPart2;

        public Network Network { get; private set; }

        public Network NetworkSuggested { get; private set; }

        public LatestPriceResultMessage LastResult { get; set; }

        public void Discover()
        {
            if (_discoveryRequestRequest != null)
                return;

            _messenger.Register<AssetPairDiscoveryResultMessage>(this, AssetPairProviderResultMessage);
            _discoveryRequestRequest = new AssetPairDiscoveryRequestMessage { Network = Network, Pair = Pair, ConversionEnabled = true, PeggedEnabled = true, ReversalEnabled = true };
            _messenger.Send(_discoveryRequestRequest);
        }

        private void AssetPairProviderResultMessage(AssetPairDiscoveryResultMessage m)
        {
            if (_discoveryRequestRequest == null || !_discoveryRequestRequest.Equals(m.RequestRequestMessage))
                return;

            _messenger.Unregister<AssetPairDiscoveryResultMessage>(this);

            var r = m.Providers;
            ProcessDiscoveryResponse(r);
        }

        private void ProcessDiscoveryResponse(AssetPairProviders r, bool isPart2 = false)
        {
            PairRequestable = r.Pair;
            IsConvertedPart1 = !isPart2 && r.Via != null;
            Providers = r;
            Network = r.Provider.Network;
            IsVerified = true;

            if (IsConvertedPart1 && !isPart2)
                ProcessConvertedPart2(r.Via);

            _messenger.Send(new InternalLatestPriceRequestVerifiedMessage(this));
        }

        private void ProcessConvertedPart2(AssetPairProviders provs)
        {
            var request = new LatestPriceRequest(Pair, provs.Provider.Network)
            {
                ConvertedOther = this,
                IsConvertedPart1 = false,
                IsConvertedPart2 = true
            };

            ConvertedOther = request;
            request.ProcessDiscoveryResponse(provs, true);
        }
    }

    public partial class LatestPriceRequest
    {
        public bool Equals(AssetPair pair, bool isConvertedPart1, bool isConvertedPart2, Network networkSuggested)
        {
            return Equals(Pair, pair) && IsConvertedPart1 == isConvertedPart1 && IsConvertedPart2 == isConvertedPart2 && (Equals(NetworkSuggested, networkSuggested) || NetworkSuggested == null && networkSuggested == null);
        }

        public bool Equals(LatestPriceRequest other)
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
            return Equals((LatestPriceRequest)obj);
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