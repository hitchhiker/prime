using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Core.Market
{
    public class VolumeDbProvider
    {
        public static VolumeDbProvider I => Lazy.Value;
        private static readonly Lazy<VolumeDbProvider> Lazy = new Lazy<VolumeDbProvider>(() => new VolumeDbProvider("VOLUMEDATA_I".GetObjectIdHashCode()));
        private readonly object _lock = new object();
        public readonly bool CanSave;

        public readonly IAggVolumeDataProvider ProviderAggVolumeData;
        public readonly IReadOnlyList<IPublicVolumeProvider> ProvidersPublicVolume;
        public readonly IReadOnlyList<IPublicPricingProvider> ProvidersPublicPricing;

        public readonly VolumeData Data;

        public VolumeDbProvider()
        {
            ProviderAggVolumeData = Networks.I.Providers.OfType<IAggVolumeDataProvider>().FirstOrDefault();
            ProvidersPublicVolume = Networks.I.Providers.OfType<IPublicVolumeProvider>().Where(x => x.IsDirect).ToList();
            ProvidersPublicPricing = Networks.I.Providers.OfType<IPublicPricingProvider>().Where(x=>x.IsDirect && x.PricingFeatures.HasVolume).ToList();

            Data = new VolumeData();
        }

        public VolumeDbProvider(ObjectId dataId) : this()
        {
            Data = PublicFast.GetCreate(dataId, () => new VolumeData());
            CanSave = true;
        }

        public NetworkPairVolume GetVolume(Network network, AssetPair pair, bool dontSave = false)
        {
            lock (_lock)
            {
                var v = Data.Get(network, pair);
                if (v != null)
                    return v;

                var canagg = ProviderAggVolumeData.NetworksSupported.Contains(network);

                var save = canagg && Data.PopulateFromAgg(ProviderAggVolumeData, pair); //the agg does bulk, so it goes up the list.
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
        
        public PublicVolumeResponse GetAllVolume(IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> pairsByNetwork, Action<Network, AssetPair> onPull = null, Action<Network, AssetPair, NetworkPairVolume> afterPull = null)
        {
            var volume = new UniqueList<NetworkPairVolume>();
            var missing = new Dictionary<Network, UniqueList<AssetPair>>();
            foreach (var network in pairsByNetwork.Keys)
            {
                var pairs = pairsByNetwork[network];

                foreach (var pair in pairs)
                {
                    onPull?.Invoke(network, pair);
                    var r = GetVolume(network, pair, true);
                    if (r == null)
                        missing.GetOrAdd(network, (k) => new UniqueList<AssetPair>()).Add(pair);
                    else
                        volume.Add(r);
                    afterPull?.Invoke(network, pair, r);
                }

                if (CanSave)
                    Data.SavePublic();
            }

            return new PublicVolumeResponse(volume , missing);
        }

        public static void Clear(ObjectId id)
        {
            PublicFast.Delete<VolumeData>(id);
        }
    }
}