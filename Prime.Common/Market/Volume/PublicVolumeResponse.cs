using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public class PublicVolumeResponse
    {
        public readonly IReadOnlyList<NetworkPairVolume> Volume;
        public readonly IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> Missing;

        public bool WasViaSingleMethod { get; set; }

        public PublicVolumeResponse(NetworkPairVolume volume)
        {
            Volume = new UniqueList<NetworkPairVolume>() { volume };
            Missing = new Dictionary<Network, IReadOnlyList<AssetPair>>();

            WasViaSingleMethod = true;
        }

        public PublicVolumeResponse(IEnumerable<PublicVolumeResponse> responses)
        {
            var list = responses.Where(x => x != null).ToList();

            var m = new Dictionary<Network, UniqueList<AssetPair>>();
            Volume = new UniqueList<NetworkPairVolume>(list.SelectMany(x => x.Volume));

            foreach (var n in list.SelectMany(x => x.Missing.Select(mm => mm.Key)).ToUniqueList())
                m.Add(n, new UniqueList<AssetPair>());

            foreach (var mi in list.SelectMany(x => x.Missing))
                m[mi.Key].AddRange(mi.Value);
        
            Missing = m.ToDictionary(x=>x.Key, y=>y.Value.AsReadOnlyList());
        }


        public PublicVolumeResponse(Network network, IEnumerable<AssetPair> missing) : this(null, network, missing) { }

        public PublicVolumeResponse(Network network, MarketPricesResult marketPricesResult) : this(marketPricesResult.MarketPrices.Select(x => x.Volume), network, marketPricesResult.MissedPairs) { }

        public PublicVolumeResponse(Network network, AssetPair pair, decimal volume24) : this(new NetworkPairVolume(network, pair, volume24)) { }

        public PublicVolumeResponse(Network network, AssetPair pair, decimal? vol24Base, decimal? vol24Quote = null) : this(new NetworkPairVolume(network, pair, vol24Base, vol24Quote)) { }

        public PublicVolumeResponse(IEnumerable<NetworkPairVolume> volume, Network network, IEnumerable<AssetPair> missing)
        {
            Volume = volume?.ToUniqueList() ?? new UniqueList<NetworkPairVolume>();
            Missing = new Dictionary<Network, IReadOnlyList<AssetPair>> {{network, missing.ToUniqueList()}};
        }

        public PublicVolumeResponse(IEnumerable<NetworkPairVolume> volume, Dictionary<Network, UniqueList<AssetPair>> missing = null)
        {
            Volume = volume.ToUniqueList();
            Missing = missing == null ? new Dictionary<Network, IReadOnlyList<AssetPair>>() : missing.ToDictionary(x => x.Key, y => y.Value.AsReadOnlyList());
        }
        
        public NetworkPairVolume GetWithReversed(Network network, AssetPair pair)
        {
            var v = Volume.FirstOrDefault(x=>x.Network.Id == network.Id && x.Pair.EqualsOrReversed(pair));
            if (v == null)
                return null;

            return v.Pair.Id == pair.Id ? v : v.Reversed;
        }
    }
}