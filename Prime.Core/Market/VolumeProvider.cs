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
        private static readonly Lazy<VolumeProvider> Lazy = new Lazy<VolumeProvider>(() => new VolumeProvider("VOLUMEDATA_I2sda".GetObjectIdHashCode()));
        private readonly object _lock = new object();
        public readonly bool CanSave;

        public VolumeProvider()
        {
            AggVolumeDataProvider = Networks.I.Providers.OfType<IAggVolumeDataProvider>().FirstOrDefault();
            Data = new VolumeData();
        }

        public VolumeProvider(ObjectId dataId) : this()
        {
            Data = PublicFast.GetCreate(dataId, () => new VolumeData());
            CanSave = true;
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

                var canagg = AggVolumeDataProvider.NetworksSupported.Contains(network);

                var save = canagg && Data.PopulateFromAgg(AggVolumeDataProvider, pair); //the agg does bulk, so it goes first.
                v = canagg ? Data.Get(network, pair) : null;
                
                if (v != null)
                    return PostProcess(v, save && !dontSave);

                save = Data.PopulateFromApi(network, pair) || save;

                v = Data.Get(network, pair);

                if (v != null)
                    return PostProcess(v, save && !dontSave);

                save = Data.PopulateFromPricing(network, pair) || save;

                return PostProcess(Data.Get(network, pair), save && !dontSave);
            }
        }

        private NetworkPairVolume PostProcess(NetworkPairVolume vol, bool mustSave)
        {
            if (mustSave && CanSave)
                Data.SavePublic();
            return vol;
        }

        public (UniqueList<NetworkPairVolume> volume, Dictionary<AssetPair, UniqueList<Network>> missing) GetAllVolume(Dictionary<Network, IReadOnlyList<AssetPair>> pairsByNetwork, Action<Network, AssetPair> onPull = null, Action<Network, AssetPair, NetworkPairVolume> afterPull = null)
        {
            var volume = new UniqueList<NetworkPairVolume>();
            var missing = new Dictionary<AssetPair, UniqueList<Network>>();
            foreach (var network in pairsByNetwork.Keys)
            {
                foreach (var pair in pairsByNetwork[network])
                {
                    onPull?.Invoke(network, pair);
                    var r = GetVolume(network, pair, true);
                    if (r == null)
                        missing.GetOrAdd(pair, (k) => new UniqueList<Network>()).Add(network);
                    else
                        volume.Add(r);
                    afterPull?.Invoke(network, pair, r);
                }

                if (CanSave)
                    Data.SavePublic();
            }

            return (volume , missing);
        }
    }
}
