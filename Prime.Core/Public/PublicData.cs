using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Core
{
    public class PublicData : ModelBase
    {
        private readonly ConcurrentDictionary<AssetPair, AssetPairData> _assetPairs = new ConcurrentDictionary<AssetPair, AssetPairData>();

        public AssetPairData GetAssetPairData(AssetPair pair)
        {
            var data = _assetPairs.GetOrAdd(pair, k =>
            {
                var id = AssetPairData.GetHash(k);
                var apd = PublicContext.I.GetCollection<AssetPairData>().FindById(id);
                if (apd != null)
                    return apd;

                var prov = Networks.I.AssetPairAggregationProviders.FirstProvider();
                var r = ApiCoordinator.GetCoinSnapshot(prov, new AssetPairDataContext(new AssetPairData(k, prov)));

                apd = r.IsNull ? new AssetPairData(k, prov) { UtcUpdated = DateTime.UtcNow, IsMissing = true } : r.Response;
                apd.Save(PublicContext.I);

                return apd;
            });

            data.Refresh(PublicContext.I);
            return data;
        }
    }
}