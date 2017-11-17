using System;
using System.Collections.Generic;
using LiteDB;

namespace Prime.Common
{
    public class VolumeDataExchange
    {
        private VolumeDataExchange() { }

        public VolumeDataExchange(Network network, AssetPair pair, Money vol24Base, Money vol24Quote)
        {
            Network = network;
            Pair = pair;
            Volume24Base = vol24Base;
            Volume24Quote = vol24Quote;
            UtcCreated = DateTime.UtcNow;
        }
        
        [Bson]
        public DateTime UtcCreated { get; private set; }

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public AssetPair Pair { get; private set; }

        public AssetPair PairN => Pair.Normalised;

        [Bson]
        public Money Volume24Base { get; private set; }

        [Bson]
        public Money Volume24Quote { get; private set; }

        public Money VolumeFor(Asset asset)
        {
            if (asset.Id == Pair.Asset1.Id)
                return Volume24Base;

            if (asset.Id == Pair.Asset2.Id)
                return Volume24Quote;

            throw new ArgumentException($"'{asset.ShortCode}' is not a member of the pair '{Pair}' in method '{nameof(VolumeFor)}'");
        }

        public VolumeDataExchange Reversed()
        {
            return new VolumeDataExchange(Network, Pair.Reversed, Volume24Quote, Volume24Base);
        }
    }
}