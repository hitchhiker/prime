﻿using System;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Prime.Core.Exchange.Rates
{
    public partial class ExchangeRateRequest : IEquatable<ExchangeRateRequest>
    {
        private readonly IMessenger _messenger;
        private readonly ExchangeRatesCoordinator _coordinator;
        public readonly AssetPair Pair;

        public ExchangeRateRequest(ExchangeRatesCoordinator coordinator, AssetPair pair, Network network = null, bool skipDiscovery = false)
        {
            _coordinator = coordinator;
            _messenger = _coordinator.Messenger;
            Network = NetworkSuggested = network;
            Pair = pair;

            if (!skipDiscovery)
                new Task(Discovery).Start();
        }

        public AssetPair PairRequestable { get; private set; }

        public bool IsVerified { get; private set; }

        public AssetPairKnownProviders Providers { get; private set; }

        public ExchangeRateRequest ConvertedOther { get; private set; }

        public bool IsConvertedPart1 { get; private set; }

        public bool IsConvertedPart2 { get; private set; }

        public bool IsConverted => IsConvertedPart1 || IsConvertedPart2;

        public Network Network { get; private set; }

        public Network NetworkSuggested { get; private set; }

        public ExchangeRateCollected LastCollected { get; set; }

        private void Discovery()
        {
            var pc = new PairProviderDiscoveryContext { Network = Network, Pair = Pair, ConversionEnabled = true, PeggedEnabled = true, ReversalEnabled = true };
            var d = new AssetPairProviderDiscovery(pc);
            var r = d.Discover();

            if (r?.Provider == null)
                return;

            ProcessDiscoveryResponse(r);
        }

        private void ProcessDiscoveryResponse(AssetPairKnownProviders r, bool isPart2 = false)
        {
            PairRequestable = r.IsReversed ? r.Pair.Reverse() : r.Pair;
            IsConvertedPart1 = !isPart2 && r.Via != null;
            Providers = r;
            Network = r.Provider.Network;
            IsVerified = true;

            if (IsConvertedPart1 && !isPart2)
                ProcessConvertedPart2(r.Via);

            _messenger.Send(new ExchangeRateRequestVerifiedMessage(this));
        }

        private void ProcessConvertedPart2(AssetPairKnownProviders provs)
        {
            var request = new ExchangeRateRequest(_coordinator, Pair, provs.Provider.Network, true)
            {
                ConvertedOther = this,
                IsConvertedPart1 = false,
                IsConvertedPart2 = true
            };

            ConvertedOther = request;
            request.ProcessDiscoveryResponse(provs, true);
        }
    }

    public partial class ExchangeRateRequest
    {
        public bool Equals(AssetPair pair, bool isConvertedPart1, bool isConvertedPart2, Network networkSuggested)
        {
            return Equals(Pair, pair) && IsConvertedPart1 == isConvertedPart1 && IsConvertedPart2 == isConvertedPart2 && (Equals(NetworkSuggested, networkSuggested) || NetworkSuggested == null && networkSuggested == null);
        }

        public bool Equals(ExchangeRateRequest other)
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
            return Equals((ExchangeRateRequest)obj);
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