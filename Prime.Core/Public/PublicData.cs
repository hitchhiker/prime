using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Prime.Utility;

namespace Prime.Core
{
    public class PublicData : ModelBase
    {
        [Bson]
        private ConcurrentDictionary<AssetPair, AssetExchangeData> AssetPairs { get; set; } = new ConcurrentDictionary<AssetPair, AssetExchangeData>();

        private ConcurrentDictionary<Asset, List<INetworkProvider>> _assetProviders;

        public ConcurrentDictionary<Asset, List<INetworkProvider>> AssetProviders => _assetProviders ?? (_assetProviders = GetAssetProviders());

        private ConcurrentDictionary<Asset, List<INetworkProvider>> GetAssetProviders()
        {
            var r = new ConcurrentDictionary<Asset, List<INetworkProvider>>();
            foreach (var a in AssetPairs.Keys.Select(x => x.Asset1).Union(AssetPairs.Keys.Select(x => x.Asset2)).ToUniqueList())
            {
                var ms = AssetPairs.Where(x=>x.Key.Asset1.ShortCode == a.ShortCode || x.Key.Asset2.ShortCode == a.ShortCode).ToList();
                var providers = ms.Select(x => x.Value).SelectMany(x => x.AllProviders).ToList();
                r.Add(a, providers);
            }
            return r;
        }

        public AssetExchangeData AssetExchangeData(AssetPair pair)
        {
            var data = AssetPairs.GetOrAdd(pair, k =>
            {
                var prov = Networks.I.AssetPairAggregationProviders.FirstProvider();
                var r = ApiCoordinator.GetCoinInfo(prov, new AggregatedCoinInfoContext(k));
                if (!r.IsNull)
                    this.Save(PublicContext.I);
                return r.Response ?? new AssetExchangeData(k, prov) {UtcUpdated = DateTime.UtcNow};
            });

            data.Refresh(PublicContext.I);
            return data;
        }
    }
}