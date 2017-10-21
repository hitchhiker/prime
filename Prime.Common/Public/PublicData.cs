using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Common
{
    public class PublicData : ModelBase
    {
        private readonly ConcurrentDictionary<AssetPair, AggregatedAssetPairData> _assetPairs = new ConcurrentDictionary<AssetPair, AggregatedAssetPairData>();

        public AggregatedAssetPairData GetAggAssetPairData(AssetPair pair)
        {
            var data = _assetPairs.GetOrAdd(pair, k =>
            {
                var id = AggregatedAssetPairData.GetHash(k);
                var apd = PublicContext.I.GetCollection<AggregatedAssetPairData>().FindById(id);
                if (apd != null)
                    return apd;

                var prov = Networks.I.AssetPairAggregationProviders.FirstProvider();
                var r = ApiCoordinator.GetCoinSnapshot(prov, new AssetPairDataContext(new AggregatedAssetPairData(k, prov)));

                apd = r.IsNull ? new AggregatedAssetPairData(k, prov) { UtcUpdated = DateTime.UtcNow, IsMissing = true } : r.Response;
                apd.Save(PublicContext.I);

                return apd;
            });

            data.Refresh(PublicContext.I);
            return data;
        }
    }
}