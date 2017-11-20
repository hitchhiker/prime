using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Utility;

namespace Prime.Core.Market
{
    public class VolumeProviderReturn
    {
        public readonly IReadOnlyList<NetworkPairVolume> Volume;
        public readonly IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> Missing;

        public VolumeProviderReturn(UniqueList<NetworkPairVolume> volume, Dictionary<Network, UniqueList<AssetPair>> missing)
        {
            Volume = volume;
            Missing = missing.ToDictionary(x => x.Key, y => y.Value.AsReadOnlyList<AssetPair>());
        }
    }
}