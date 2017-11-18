using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime.Common;
using Prime.Utility;

namespace Prime.Core.Market
{
    public class VolumeProvider
    {
        private VolumeProvider()
        {
            AggVolumeDataProvider = Networks.I.Providers.OfType<IAggVolumeDataProvider>().FirstOrDefault();
        }

        public static VolumeProvider I => Lazy.Value;
        private static readonly Lazy<VolumeProvider> Lazy = new Lazy<VolumeProvider>(()=>new VolumeProvider());

        public IAggVolumeDataProvider AggVolumeDataProvider;

        public NetworkPairVolume GetVolume(Network network, AssetPair pair)
        {
            var data = PublicFast.GetCreate<NetworkPairVolumeData>(pair.Id);

            var dbn = data.ByNetworks.Get(network);
            if (dbn != null)
                return dbn;

            var apidata = GetVolumeData(network, pair);
            var rs = apidata?.ByNetworks.Get(network);
        }

        private NetworkPairVolume GetVolumeData(Network network, AssetPair pair)
        {
            var ntp = network.Providers.OfType<IAssetPairVolumeProvider>().FirstOrDefault(x => x.IsDirect);
            if (ntp != null)
            {
                var r = ApiCoordinator.GetAssetPairVolume(ntp, new VolumeContext(pair));
                if (!r.IsNull)
                    return r.Response;
            }
            return GetVolumeDataAgg(pair)?.ByNetworks.Get(network);
        }

        private NetworkPairVolumeData GetVolumeDataAgg(AssetPair pair)
        {
            if (AggVolumeDataProvider == null)
                return null;

            Console.WriteLine("Getting volume data for: " + pair);

            var apir = ApiCoordinator.GetAggVolumeData(AggVolumeDataProvider, new AggVolumeDataContext(pair));

            return apir.IsNull ? null : apir.Response;
        }
    }
}
