using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class PairVolumeData : ModelBase, ISerialiseAsObject
    {
        public PairVolumeData(AssetPair pair)
        {
            Pair = pair.Normalised;
            Id = Pair.Id;
        }

        public void Add(NetworkPairVolumeData data, bool isAgg, bool isReversed)
        {
            if (isAgg)
                UtcLastAgg = DateTime.UtcNow;

            if (isReversed)
                if (isAgg)
                    UtcLastAggReversed = DateTime.UtcNow;
                else
                    UtcLastReversed = DateTime.UtcNow;

            var list = isAgg ? aggs : direct;
            foreach (var v in data)
                list.Add(v);
        }

        public NetworkPairVolume Get(Network network)
        {
            return ByNetworks.Get(network) ?? AggsByNetworks.Get(network);
        }

        [Bson]
        public AssetPair Pair { get; private set; }

        [Bson]
        public DateTime UtcLastAgg { get; set; }

        [Bson]
        public DateTime UtcLastAggReversed { get; set; }

        [Bson]
        public DateTime UtcLastReversed { get; set; }

        public bool IsEmpty => aggs.Count == 0 && direct.Count==0;

        private Dictionary<Network, NetworkPairVolume> _byNetworks;
        public Dictionary<Network, NetworkPairVolume> ByNetworks => _byNetworks ?? (_byNetworks = direct.GroupBy(x => x.Network).ToDictionary(x => x.Key, y => y.FirstOrDefault()));

        private Dictionary<Network, NetworkPairVolume> _aggsbyNetworks;
        public Dictionary<Network, NetworkPairVolume> AggsByNetworks => _aggsbyNetworks ?? (_aggsbyNetworks = aggs.GroupBy(x => x.Network).ToDictionary(x => x.Key, y => y.FirstOrDefault()));

        [Bson]
        private List<NetworkPairVolume> aggs { get; set; } = new List<NetworkPairVolume>();

        [Bson]
        private List<NetworkPairVolume> direct { get; set; } = new List<NetworkPairVolume>();
    }
}