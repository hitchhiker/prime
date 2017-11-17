using System;

namespace Prime.Common
{
    public class VolumeData
    {
        public VolumeData(Network network, AssetPair pair, Money volume24, DateTime? utcCreated = null)
        {
            UtcCreated = utcCreated ?? DateTime.UtcNow;
            Pair = pair;
            Network = network;
            Volume24 = volume24;
        }

        [Bson]
        public Money Volume24 { get; private set; }

        [Bson]
        public DateTime UtcCreated { get; private set; }

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public AssetPair Pair { get; private set; }

        public AssetPair PairN => Pair.Normalised;
    }
}