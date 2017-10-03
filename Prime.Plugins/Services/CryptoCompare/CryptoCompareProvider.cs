using System;
using System.Linq;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Plugins.Services.CryptoCompare;
using Prime.Utility;

// BUG: unable to move to correct namespace because of dynamic assembly loading.
namespace plugins
{
    public class CryptoCompareProvider : CryptoCompareBase, IAssetPairAggregationProvider, ILatestPriceAggregationProvider
    {
        public override string Name => "CCCAGG";

        public override string AggregatorName => null;

        public async Task<AssetExchangeData> GetCoinInfoAsync(AggregatedCoinInfoContext context)
        {
            var cd = new AssetExchangeData(context.Pair, this);
            await RefreshCoinInfoAsync(cd);
            return cd;
        }

        public override string Title => "CryptoCompare Aggregator";

        public async Task<bool> RefreshCoinInfoAsync(AssetExchangeData assetData)
        {
            var pair = assetData.AssetPair;
            var api = GetApi<ICryptoCompareApi>(true);
            var apir = await api.GetCoinSnapshotAsync(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this));

            if (apir.IsError() || apir.Data == null)
                return false;

            var d = apir.Data;

            assetData.UtcUpdated = DateTime.UtcNow;
            assetData.ProofType = d.ProofType;
            assetData.TotalCoinsMined = d.TotalCoinsMined.ToDouble();
            assetData.NetHashesPerSecond = d.NetHashesPerSecond.ToDouble();
            assetData.BlockNumber = d.BlockNumber.ToLong();
            assetData.Algorithm = d.Algorithm;
            assetData.BlockReward = d.BlockReward.ToDouble();
            assetData.AggregatedEntry = Convert(d.AggregatedData);

            assetData.Exchanges.Clear();

            foreach (var v in apir.Data.Exchanges)
            {
                var nd = Convert(v);
                if (nd.Network == null)
                    continue;
                assetData.Exchanges.Add(nd);
            }

            return true;
        }

        private AssetExchangeEntry Convert(CryptoCompareSchema.CoinSnapshotDataBlock r)
        {
            var n = Networks.I.FirstOrDefault(x => string.Equals(r.MARKET, x.Name, StringComparison.OrdinalIgnoreCase));
            var ce = new AssetExchangeEntry(n)
            {
                Type = r.TYPE,
                Flags = r.FLAGS,
                Price = r.PRICE.ToDouble(),
                UtcLastUpdate = DateTime.UtcNow,
                LastVolume = r.LASTVOLUME.ToDouble(),
                LastVolumeTo = r.LASTVOLUMETO.ToDouble(),
                Volume24Hour = r.VOLUME24HOUR.ToDouble(),
                Volume24HourTo = r.VOLUME24HOURTO.ToDouble(),
                Open24Hour = r.OPEN24HOUR.ToDouble(),
                High24Hour = r.HIGH24HOUR.ToDouble(),
                Low24Hour = r.LOW24HOUR.ToDouble()
            };
            return ce;
        }
    }
}
