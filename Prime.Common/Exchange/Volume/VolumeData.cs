using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Nito.AsyncEx;
using Prime.Core.Market;
using Prime.Utility;

namespace Prime.Common
{
    public class VolumeData : ModelBase, ISerialiseAsObject, IReadOnlyList<NetworkPairVolume>
    {
        public VolumeData() { }

        private void AddRange(IEnumerable<NetworkPairVolume> volumes)
        {
            if (volumes == null)
                return;

            foreach (var v in volumes)
                Add(v);
        }

        private void Add(NetworkPairVolume volume)
        {
            if (volume.Network == null)
                throw new Exception("Network was null");

            if (!volume.Pair.IsNormalised)
            {
                Add(volume.Reversed);
                return;
            }

            var pair = volume.Pair;

            Data.RemoveAll(x => x.Network == null);

            var e = Data.FirstOrDefault(x => x.Pair.Id == pair.Id && x.Network.Id == volume.Network.Id);

            if (!ShouldReplace(e, volume))
                return;

            Data.Add(volume, true);
            _byNetworks = null;
            _byPair = null;
        }

        private bool ShouldReplace(NetworkPairVolume old, NetworkPairVolume next)
        {
            if (old == null)
                return true;

            if ((old.HasVolume24Base && !next.HasVolume24Base) || (old.HasVolume24Quote && !next.HasVolume24Quote) || next.UtcCreated < old.UtcCreated)
                return false;

            return true;
        }

        private Dictionary<Network, IReadOnlyList<NetworkPairVolume>> _byNetworks;
        public IReadOnlyDictionary<Network, IReadOnlyList<NetworkPairVolume>> ByNetworks => _byNetworks ?? (_byNetworks = Data.GroupBy(x => x.Network).ToDictionary(x => x.Key, y => y.AsReadOnlyList()));

        private Dictionary<AssetPair, IReadOnlyList<NetworkPairVolume>> _byPair;
        public IReadOnlyDictionary<AssetPair, IReadOnlyList<NetworkPairVolume>> ByPair => _byPair ?? (_byPair = Data.GroupBy(x => x.Pair).ToDictionary(x => x.Key, y => y.AsReadOnlyList()));

        [Bson]
        private Dictionary<ObjectId, DateTime> UtcAggLatest { get; set; } = new Dictionary<ObjectId, DateTime>();

        [Bson]
        private Dictionary<ObjectId, DateTime> UtcDirectLatest { get; set; } = new Dictionary<ObjectId, DateTime>();

        [Bson]
        private UniqueList<NetworkPairVolume> Data { get; set; } = new UniqueList<NetworkPairVolume>();

        public IReadOnlyList<NetworkPairVolume> GetNormalised(Network network)
        {
            return ByNetworks.Get(network);
        }

        public IReadOnlyList<NetworkPairVolume> GetNormalised(AssetPair normalised)
        {
            if (!normalised.IsNormalised)
                throw new Exception($"{nameof(AssetPair)} must be normalied.");

            return ByPair.Get(normalised);
        }

        public NetworkPairVolume GetNormalised(Network network, AssetPair pair)
        {
            var vol = GetNormalised(pair.Normalised)?.FirstOrDefault(x => x.Network?.Id == network.Id);

            if (vol == null)
                return null;

            return pair.IsNormalised ? vol : vol.Reversed;
        }

        public bool PopulateFromApi(Network network)
        {
            var ids = network.Id;

            if (UtcDirectLatest.Get(ids).IsWithinTheLast(TimeSpan.FromDays(10)))
                return false;

            UtcDirectLatest.Add(ids, DateTime.UtcNow);

            var vp = AsyncContext.Run(() => VolumeProvider.I.GetAsync(network));

            if (vp == null)
                return true;

            AddRange(vp.Volume);

            return true;
        }

        public bool PopulateFromAgg(IAggVolumeDataProvider prov, AssetPair pair)
        {
            if (prov == null)
                return false;

            var norm = pair.Normalised;

            if (UtcAggLatest.Get(norm.Id).IsWithinTheLast(TimeSpan.FromDays(10)))
                return false;

            var aggn = GetVolumeDataAgg(prov, norm);
            var aggr = GetVolumeDataAgg(prov, norm.Reversed);
            
            if (aggr!=null)
                AddRange(aggr.Volume);
            if (aggn!=null)
                AddRange(aggn.Volume);

            UtcAggLatest.Add(norm.Id, DateTime.UtcNow);
            return true;
        }

        private PublicVolumeResponse GetVolumeDataAgg(IAggVolumeDataProvider prov, AssetPair pair)
        {
            var apir = ApiCoordinator.GetAggVolumeData(prov, new AggVolumeDataContext(pair));
            return apir.IsNull ? null : apir.Response;
        }

        public IEnumerator<NetworkPairVolume> GetEnumerator() => Data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) Data).GetEnumerator();

        public int Count => Data.Count;

        public NetworkPairVolume this[int index] => Data[index];
    }
}