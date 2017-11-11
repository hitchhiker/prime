using System;
using LiteDB;
using Prime.Utility;

namespace Prime.Common
{
    public class AssetPairData : ModelBase
    {
        private AssetPairData() { }

        public AssetPairData(Network network, AssetPairs pairs)
        {
            Network = network;
            Id = GetHash(Network);
            Pairs = pairs;
        }

        public static ObjectId GetHash(Network network)
        {
            return ("assetpairnetworkdata:" + network.Id).GetObjectIdHashCode();
        }

        [Bson]
        public Network Network { get; private set; }

        [Bson]
        public AssetPairs Pairs { get; private set; }

        public async void RefreshAsync()
        {
            var prov = Network.Providers.FirstProviderOf<IAssetPairsProvider>();
            if (prov==null)
                throw new Exception($"Cannot 'refresh' {nameof(AssetPairData)} for {Network.Name} as a {nameof(IAssetPairsProvider)} cannot be located.");

            var pairs = await ApiCoordinator.GetAssetPairsAsync(prov).ConfigureAwait(false);
            if (pairs.IsFailed)
                return;

            Pairs = pairs.Response;
        }
    }
}