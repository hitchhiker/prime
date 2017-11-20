using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
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
            var e = _data.FirstOrDefault(x => x.Pair.Id == pair.Id && x.Network.Id == volume.Network.Id);

            if (!ShouldReplace(e, volume))
                return;

            _data.Add(volume, true);
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
        public IReadOnlyDictionary<Network, IReadOnlyList<NetworkPairVolume>> ByNetworks => _byNetworks ?? (_byNetworks = _data.GroupBy(x => x.Network).ToDictionary(x => x.Key, y => y.AsReadOnlyList()));

        private Dictionary<AssetPair, IReadOnlyList<NetworkPairVolume>> _byPair;
        public IReadOnlyDictionary<AssetPair, IReadOnlyList<NetworkPairVolume>> ByPair => _byPair ?? (_byPair = _data.GroupBy(x => x.Pair).ToDictionary(x => x.Key, y => y.AsReadOnlyList()));

        [Bson]
        private Dictionary<ObjectId, DateTime> _utcAggLatest { get; set; } = new Dictionary<ObjectId, DateTime>();

        [Bson]
        private Dictionary<string, DateTime> _utcDirectLatest { get; set; } = new Dictionary<string, DateTime>();

        [Bson]
        private Dictionary<string, DateTime> _utcPricingLatest { get; set; } = new Dictionary<string, DateTime>();

        [Bson]
        private UniqueList<NetworkPairVolume> _data { get; set; } = new UniqueList<NetworkPairVolume>();

        public IReadOnlyList<NetworkPairVolume> Get(Network network)
        {
            return ByNetworks.Get(network);
        }

        public IReadOnlyList<NetworkPairVolume> Get(AssetPair normalised)
        {
            if (!normalised.IsNormalised)
                throw new Exception($"{nameof(AssetPair)} must be normalied.");

            return ByPair.Get(normalised);
        }

        public NetworkPairVolume Get(Network network, AssetPair pair)
        {
            var vol = Get(pair.Normalised)?.FirstOrDefault(x => x.Network.Id == network.Id);

            if (vol == null)
                return null;

            return pair.IsNormalised ? vol : vol.Reversed;
        }
        
        public bool PopulateFromApi(Network network, AssetPair pair, bool isRecurse = false)
        {
            if (!isRecurse)
            {
                var ids = network.Id + ":" + pair.Normalised.Id;

                if (_utcDirectLatest.Get(ids).IsWithinTheLast(TimeSpan.FromDays(10)))
                    return false;

                _utcDirectLatest.Add(ids, DateTime.UtcNow);
            }

            var ntp = network.AssetPairVolumeProviders.FirstDirectProvider();
            if (ntp == null)
                return true;
            
            var r = ApiCoordinator.GetPublicVolume(ntp, new VolumeContext(pair));
            if (!r.IsNull)
                Add(r.Response);
            else if (pair.IsNormalised)
                return PopulateFromApi(network, pair.Reversed, true) || true;

            return true;
        }

        public bool PopulateFromPricing(Network network, AssetPair pair, bool isRecurse = false)
        {
            if (!isRecurse)
            {
                var ids = network.Id + ":" + pair.Normalised.Id;

                if (_utcPricingLatest.Get(ids).IsWithinTheLast(TimeSpan.FromDays(10)))
                    return false;

                _utcPricingLatest.Add(ids, DateTime.UtcNow);
            }

            var ntp = network.PublicPriceProviders.Where(x=>x.PricingFeatures.HasVolume).FirstDirectProvider();
            if (ntp == null)
                return true;

            var r = ApiCoordinator.GetPricing(ntp, new PublicPriceContext(pair));
            if (!r.IsNull && r.Response.FirstPrice?.Volume!=null)
                Add(r.Response.FirstPrice.Volume);
            else if (pair.IsNormalised)
                return PopulateFromPricing(network, pair.Reversed, true) || true;

            return true;
        }

        public bool PopulateFromAgg(IAggVolumeDataProvider prov, AssetPair pair)
        {
            if (prov == null)
                return false;

            var norm = pair.Normalised;

            if (_utcAggLatest.Get(norm.Id).IsWithinTheLast(TimeSpan.FromDays(10)))
                return false;

            var aggn = GetVolumeDataAgg(prov, norm);
            var aggr = GetVolumeDataAgg(prov, norm.Reversed);
            
            AddRange(aggr);
            AddRange(aggn);

            _utcAggLatest.Add(norm.Id, DateTime.UtcNow);
            return true;
        }

        private NetworkPairVolumeData GetVolumeDataAgg(IAggVolumeDataProvider prov, AssetPair pair)
        {
            var apir = ApiCoordinator.GetAggVolumeData(prov, new AggVolumeDataContext(pair));
            return apir.IsNull ? null : apir.Response;
        }

        public IEnumerator<NetworkPairVolume> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _data).GetEnumerator();

        public int Count => _data.Count;

        public NetworkPairVolume this[int index] => _data[index];
    }
}