using System;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Core.Prices.Latest.Messages;
using Prime.Utility;
using Prime.Utility.Misc;

namespace Prime.Core.Prices.Latest
{
    internal sealed partial class Request : IEquatable<Request>
    {
        private readonly IMessenger _messenger = DefaultMessenger.I.Default;
        public readonly AssetPair Pair;

        public Request(AssetPair pair, Network network = null)
        {
            Network = NetworkSuggested = network;
            Pair = pair;
        }

        private volatile AssetPairDiscoveryRequestMessage _discoveryRequest;

        public AssetPair PairForProvider { get; private set; }

        public bool IsVerified { get; private set; }

        public AssetPairNetworks Providers { get; private set; }

        public Request ConvertedOther { get; private set; }

        public bool IsConvertedPart1 { get; private set; }

        public bool IsConvertedPart2 { get; private set; }

        public bool IsConverted => IsConvertedPart1 || IsConvertedPart2;

        public Network Network { get; private set; }

        public Network NetworkSuggested { get; private set; }

        public LatestPriceResultMessage LastResult { get; set; }

        public LatestPrice LastPrice { get; set; }

        public void Discover()
        {
            if (_discoveryRequest != null || IsVerified)
                return;

            _messenger.Register<AssetPairDiscoveryResultMessage>(this, AssetPairProviderResultMessage);
            _discoveryRequest = new AssetPairDiscoveryRequestMessage { Network = Network, Pair = Pair, ConversionEnabled = true, PeggedEnabled = true, ReversalEnabled = true };
            _messenger.Send(_discoveryRequest);
        }

        private void AssetPairProviderResultMessage(AssetPairDiscoveryResultMessage m)
        {
            if (_discoveryRequest == null || !_discoveryRequest.Equals(m.RequestRequestMessage))
                return;

            _messenger.Unregister<AssetPairDiscoveryResultMessage>(this);

            var r = m.Networks;

            if (!r.Has<IPublicPriceSuper>())
                return;

            ProcessDiscoveryResponse(r);
        }

        private void ProcessDiscoveryResponse(AssetPairNetworks r, bool isPart2 = false)
        {
            PairForProvider = r.Pair;
            IsConvertedPart1 = !isPart2 && r.ConversionPart2 != null;
            Providers = r;
            Network = r.Network<IPublicPriceSuper>();
            IsVerified = true;

            if (IsConvertedPart1 && !isPart2)
                ProcessConvertedPart2(r.ConversionPart2);

            _messenger.Send(new VerifiedMessage(this));
        }

        private void ProcessConvertedPart2(AssetPairNetworks networks)
        {
            var request = new Request(Pair, networks.Network<IPublicPriceSuper>())
            {
                ConvertedOther = this,
                IsConvertedPart1 = false,
                IsConvertedPart2 = true
            };

            ConvertedOther = request;
            request.ProcessDiscoveryResponse(networks, true);
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