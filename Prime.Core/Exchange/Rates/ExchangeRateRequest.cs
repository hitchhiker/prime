using System;
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

        public ExchangeRateRequest(ExchangeRatesCoordinator coordinator, AssetPair pair, Network network = null)
        {
            _coordinator = coordinator;
            _messenger = _coordinator.Messenger;
            Pair = pair;

            new Task(Discovery).Start();
        }

        public bool IsVerified { get; private set; }

        public AssetPairKnownProviders Providers { get; private set; }

        public Network Network { get; private set; }

        private void Discovery()
        {
            var d = new AssetPairProviderDiscovery(new PairProviderDiscoveryContext { ConversionEnabled = true, PeggedEnabled = true, Pair = Pair });
            var r = d.FindProviders();
            if (r?.Provider == null)
                return;

            Providers = r;
            Network = r.Provider.Network;
            IsVerified = true;
            _messenger.Send(new ExchangeRateRequestVerifiedMessage(this));
        }
    }

    public partial class ExchangeRateRequest
    {
        public bool Equals(ExchangeRateRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Pair, other.Pair) && Equals(Network, other.Network);
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