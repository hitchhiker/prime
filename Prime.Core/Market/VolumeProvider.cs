using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Core.Market
{
    public class VolumeProvider
    {
        public static VolumeProvider I => Lazy.Value;
        private static readonly Lazy<VolumeProvider> Lazy = new Lazy<VolumeProvider>(() => new VolumeProvider("VOLUMEDATA_I".GetObjectIdHashCode()));
        private readonly object _lock = new object();

        public VolumeProvider(ObjectId dataId)
        {
            AggVolumeDataProvider = Networks.I.Providers.OfType<IAggVolumeDataProvider>().FirstOrDefault();
            Data = PublicFast.GetCreate(dataId, () => new VolumeData());
        }

        public IAggVolumeDataProvider AggVolumeDataProvider;
        public VolumeData Data;

        public NetworkPairVolume GetVolume(Network network, AssetPair pair, bool dontSave = false)
        {
            lock (_lock)
            {
                var v = Data.Get(network, pair);
                if (v != null)
                    return v;

                var save = Data.PopulateFromAgg(AggVolumeDataProvider, pair); //the agg does bulk, so it goes first.
                v = Data.Get(network, pair);
                
                if (v != null)
                    return PostProcess(v, save && !dontSave);

                save = Data.PopulateFromApi(network, pair) || save;

                return PostProcess(Data.Get(network, pair), save && !dontSave);
            }
        }

        private NetworkPairVolume PostProcess(NetworkPairVolume vol, bool mustSave)
        {
            if (mustSave)
                Data.SavePublic();
            return vol;
        }

        public (UniqueList<NetworkPairVolume> volume, Dictionary<AssetPair, UniqueList<Network>> missing) GetAllVolume(IEnumerable<AssetPair> pairs, IEnumerable<Network> networks, Action<Network, AssetPair> onPull = null, Action<Network, AssetPair, NetworkPairVolume> afterPull = null)
        {
            var pairsl = pairs.AsList();
            var networksl = networks.AsList();
            var volume = new UniqueList<NetworkPairVolume>();
            var missing = new Dictionary<AssetPair, UniqueList<Network>>();
            foreach (var network in networksl)
            {
                foreach (var pair in pairsl)
                {
                    onPull?.Invoke(network, pair);
                    var r = GetVolume(network, pair, true);
                    if (r == null)
                        missing.GetOrAdd(pair, (k) => new UniqueList<Network>()).Add(network);
                    else
                        volume.Add(r);
                    afterPull?.Invoke(network, pair, r);
                }

                Data.SavePublic();
            }

            return (volume , missing);
        }
    }
}
