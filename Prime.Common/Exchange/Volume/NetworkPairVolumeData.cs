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

        /// <summary>
        /// return true if an insert occured
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool AddRange(NetworkPairVolumeData data)
        {
            var i = false;
            foreach (var d in data)
                i = i || Add(d);
            return i;
        }

        /// <summary>
        /// return true if an insert occured
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public bool Add(NetworkPairVolume d)
        {
            if (d.Network == null || !Networks.I.KnownNetworks.Contains(d.Network))
                return false;

            if (!d.Pair.EqualsOrReversed(Pair))
                throw new ArgumentException($"Cannot add {nameof(NetworkPairVolume)} object to {nameof(NetworkPairVolumeData)} as it has the wrong {nameof(AssetPair)} '{d.Pair}'");

            Data.RemoveAll(x => x.Network.Id == d.Network.Id);
            Data.Add(d.Pair.Reversed.Id == Pair.Id ? d.Reversed : d);

            _byNetworks = null;
            return true;
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