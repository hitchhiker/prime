using System;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetPairData : ModelBase
    {
        public AssetPairData(IAssetPairsProvider provider)
        {
            Network = provider.Network;
        }

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public AssetPairs Pairs { get; private set; }

        public async void Refresh()
        {
            var prov = Network.Providers.FirstProviderOf<IAssetPairsProvider>();
            if (prov==null)
                throw new Exception($"Cannot 'refresh' {nameof(AssetPairData)} for {Network.Name} as a {nameof(IAssetPairsProvider)} cannot be located.");

            var pairs = await ApiCoordinator.GetAssetPairsAsync(prov);
            if (pairs.IsFailed)
                return;

            Pairs = pairs.Response;
        }
    }
}