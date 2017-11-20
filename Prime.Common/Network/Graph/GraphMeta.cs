using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public class GraphMeta
    {
        public GraphMeta() { }

        public GraphMeta(NetworkContext context)
        {
            BadCoins = context.BadCoins?.ToList();
            BadNetworks = context.BadNetworks?.ToList();
        }

        public readonly IReadOnlyList<Asset> HighVolumeAssets = "USDT,EUR,USD,JPY5:".ToAssetsCsvRaw();
        public readonly IReadOnlyList<Asset> WithdrawalAssetsBlocked = "USD,EUR,JPY,KRW,CAD,AUD,GBP".ToAssetsCsvRaw();
        public readonly IReadOnlyList<AssetPair> AssumeHighVolume = "BTC_ETH,BTC:USD,BTC_XRP,BTC_LTC,BTC_USD,BTC_USDT,USDT_XRP".ToAssetPairsCsvRaw();

        public readonly List<Asset> BadCoins;
        public readonly List<Network> BadNetworks;
    }
}