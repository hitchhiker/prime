using System;
using System.Collections.Generic;
using LiteDB;

namespace Prime.Common
{
    public class NetworkPairVolume : IEquatable<NetworkPairVolume>
    {
        private NetworkPairVolume() { }

        private NetworkPairVolume(Network network, AssetPair pair)
        {
            UtcCreated = DateTime.UtcNow;
            Network = network;
            Pair = pair;
        }

        public NetworkPairVolume(Network network, AssetPair pair, decimal volume24) : this(network, pair)
        {
            HasVolume24Base = true;
            Volume24Base = new Money(volume24, pair.Asset1);
        }

        public NetworkPairVolume(Network network, AssetPair pair, decimal? vol24Base, decimal? vol24Quote = null) : this(network, pair)
        {
            HasVolume24Base = vol24Base != null;
            HasVolume24Quote = vol24Quote != null;

            if (HasVolume24Base)
                Volume24Base = new Money(vol24Base.Value, pair.Asset1);

            if (HasVolume24Quote)
                Volume24Quote = new Money(vol24Quote.Value, pair.Asset2);
        }

        public NetworkPairVolume(Network network, Money vol24Base, Money vol24Quote) : this(network, new AssetPair(vol24Base.Asset, vol24Quote.Asset))
        {
            Volume24Base = vol24Base;
            Volume24Quote = vol24Quote;
        }

        public AssetPair PairN => Pair.Normalised;

        [Bson]
        public VolumeSource Source { get; set; }

        [Bson]
        public DateTime UtcCreated { get; private set; }

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public AssetPair Pair { get; private set; }

        [Bson]
        public Money Volume24Base { get; private set; }

        [Bson]
        public Money Volume24Quote { get; private set; }

        [Bson]
        public Money? Volume24Btc { get; private set; }

        [Bson]
        public bool HasVolume24Base { get; private set; }

        [Bson]
        public bool HasVolume24Quote { get; private set; }

        [Bson]
        public bool HasVolume24Btc { get; private set; }
        
        public Money Volume24 => HasVolume24Base ? Volume24Base : Volume24Quote;

        public bool ApplyBtcVolume(IEnumerable<MarketPrice> prices)
        {
            var failed = false;

            if (HasVolume24Base)
                failed = (Volume24Btc = prices.FxConvert(Volume24Base, Asset.Btc)) == null;

            HasVolume24Btc = Volume24Btc != null;

            if (HasVolume24Quote)
                failed = (Volume24Btc = prices.FxConvert(Volume24Quote, Asset.Btc)) == null || failed;

            HasVolume24Btc = Volume24Btc != null;

            return !failed;
        }

        public Money VolumeFor(Asset asset)
        {
            if (asset.Id == Pair.Asset1.Id)
                return Volume24Base;

            if (asset.Id == Pair.Asset2.Id)
                return Volume24Quote;

            throw new ArgumentException($"'{asset.ShortCode}' is not a member of the pair '{Pair}' in method '{nameof(VolumeFor)}'");
        }
        
        private NetworkPairVolume _reversed;
        public NetworkPairVolume Reversed => _reversed ?? (_reversed = new NetworkPairVolume(Network, Pair.Reversed, HasVolume24Quote ? Volume24Quote : (decimal?)null, HasVolume24Base ? Volume24Base : (decimal?)null)
        {
            UtcCreated = UtcCreated, _reversed = this,
            Volume24Btc = Volume24Btc,
            HasVolume24Btc = HasVolume24Btc
        });

        public bool Equals(NetworkPairVolume other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Network?.Id, other.Network?.Id) && Equals(Pair.Id, other.Pair.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NetworkPairVolume) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Network != null ? Network.Id.GetHashCode() : 0) * 397) ^ (Pair != null ? Pair.Id.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{Network?.Name} {Pair} {Volume24}";
        }
    }
}