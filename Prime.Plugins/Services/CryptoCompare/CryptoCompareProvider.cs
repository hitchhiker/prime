using System;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Plugins.Services.CryptoCompare;
using Prime.Utility;

// BUG: unable to move to correct namespace because of dynamic assembly loading.
namespace plugins
{
    public class CryptoCompareProvider : CryptoCompareBase, IAssetPairAggregationProvider, ILatestPriceAggregationProvider
    {
        public override string Name => "CCCAGG";

        public override string AggregatorName => null;

        public async Task<AggregatedAssetPairData> GetCoinSnapshotAsync(AssetPairDataContext context)
        {
            var assetData = context.Document;
            if (assetData == null)
                throw new ArgumentException(nameof(context.Document) + " was null.");

            var pair = context.Pair;
            var api = GetApi<ICryptoCompareApi>(true);
            var apir = await api.GetCoinSnapshotAsync(pair.Asset1.ToRemoteCode(this), pair.Asset2.ToRemoteCode(this));

            if (apir.IsError() || apir.Data == null)
                return null;

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
                if (nd.Network == null || nd.Flags == 4)
                    continue;
                assetData.Exchanges.Add(nd);
            }

            return assetData;
        }

        public override string Title => "CryptoCompare Aggregator";


        public async Task<LatestPrice> GetPriceAsync(PublicPriceContext context)
        {
            var r = await base.GetAssetPricesAsync(context);
            return r.FirstOrDefault();
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

        private AssetPairByNetwork _cachedApn;
        private DateTime _cachedApnUtc;
        private readonly object _cachedApnLock = new object();

        public async Task<AssetPairByNetwork> GetAssetPairsAllNetworksAsync()
        {
            var task = new Task<AssetPairByNetwork>(() =>
            {
                lock (_cachedApnLock)
                {
                    if (_cachedApn != null && _cachedApnUtc.IsWithinTheLast(TimeSpan.FromHours(12)))
                        return _cachedApn;

                    var api = GetApi<ICryptoCompareApi>();
                    var r = AsyncContext.Run(() => api.GetAssetPairsAllExchanges());
                    var apn = new AssetPairByNetwork();

                    foreach (var e in r)
                    {
                        var aps = new AssetPairs();
                        foreach (var i in e.Value)
                        {
                            var a = i.Key.ToAssetRaw();
                            foreach (var q in i.Value)
                                aps.Add(new AssetPair(a, q.ToAssetRaw()));
                        }
                        apn.Add(Networks.I.Get(e.Key), aps);
                    }
                    _cachedApnUtc = DateTime.UtcNow;
                    return _cachedApn = apn;
                }
            });
            task.Start();

            return await task;
        }
    }
}
