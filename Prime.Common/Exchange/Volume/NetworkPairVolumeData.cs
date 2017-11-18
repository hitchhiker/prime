using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace Prime.Common
{
    public class NetworkPairVolumeData : ModelBase, IReadOnlyList<NetworkPairVolume>, ISerialiseAsObject
    {
        private NetworkPairVolumeData() { }

        public NetworkPairVolumeData(AssetPair pair)
        {
            Pair = pair;
            Id = pair.Id;
        }

        public bool IsEmpty => Data.Count==0;

        [Bson]
        public AssetPair Pair { get; private set; }

        [Bson]
        private List<NetworkPairVolume> Data { get; set; } = new List<NetworkPairVolume>();

        public void AddRange(NetworkPairVolumeData data)
        {
            foreach (var d in data)
                Add(d);
        }

        public void Add(NetworkPairVolume d)
        {
            if (d.Network == null)
                return;

            if (!d.Pair.EqualsOrReversed(Pair))
                throw new ArgumentException($"Cannot add {nameof(NetworkPairVolume)} object to {nameof(NetworkPairVolumeData)} as it has the wrong {nameof(AssetPair)} '{d.Pair}'");

            Data.RemoveAll(x => x.Network.Id == d.Network.Id);
            Data.Add(d.Pair.Reversed.Id == Pair.Id ? d.Reversed() : d);

            _byNetworks = null;
        }

        private Dictionary<Network, NetworkPairVolume> _byNetworks;
        public Dictionary<Network, NetworkPairVolume> ByNetworks => _byNetworks ?? (_byNetworks = Data.GroupBy(x=>x.Network).ToDictionary(x=> x.Key, y=>y.FirstOrDefault()));

        public IEnumerator<NetworkPairVolume> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Data).GetEnumerator();
        }

        public int Count => Data.Count;

        public NetworkPairVolume this[int index] => Data[index];
    }
}