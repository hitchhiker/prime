using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class AssetPairData : ModelBase
    {
        private AssetPairData() { }

        internal AssetPairData(AssetPair pair, INetworkProvider provider)
        {
            ProviderId = provider.Id;
            AssetPair = pair;
            Id = GetHash(pair);
        }

        public static ObjectId GetHash(AssetPair pair)
        {
            return ("assetpair:" + pair.Asset1.ShortCode + ":" + pair.Asset2.ShortCode).GetObjectIdHashCode(true, true);
        }

        public void Refresh(IDataContext context)
        {
            if (IsFresh)
                return;

            if (AggregationProvider == null)
                return;

            var r = ApiCoordinator.GetCoinSnapshot(AggregationProvider, new AssetPairDataContext(this));
            if (r.Success)
                this.Save(context);
        }

        private IAssetPairAggregationProvider _aggregationProvider;
        public IAssetPairAggregationProvider AggregationProvider
        {
            get { return _aggregationProvider ?? (_aggregationProvider = Networks.I.AssetPairAggregationProviders.FirstOrDefault(x => x.Id == ProviderId)); }
        }

        public bool IsFresh => UtcUpdated.IsWithinTheLast(TimeSpan.FromDays(30));

        [Bson]
        public DateTime UtcUpdated { get; set; }

        [Bson]
        public bool IsMissing { get; set; }

        [Bson]
        public ObjectId ProviderId { get; private set; }

        [Bson]
        public AssetPair AssetPair { get; private set; }

        [Bson]
        public string Algorithm { get; set; }

        [Bson]
        public string ProofType { get; set; }

        [Bson]
        public long BlockNumber { get; set; }

        [Bson]
        public double NetHashesPerSecond { get; set; }

        [Bson]
        public double TotalCoinsMined { get; set; }

        [Bson]
        public double BlockReward { get; set; }

        [Bson]
        public AssetExchangeEntry AggregatedEntry { get; set; }

        [Bson]
        public UniqueList<AssetExchangeEntry> Exchanges { get; private set; } = new UniqueList<AssetExchangeEntry>();

        public IReadOnlyList<Network> AllNetworks => Exchanges.Select(x => x.Network).ToUniqueList();

        public IReadOnlyList<INetworkProvider> AllProviders => AllNetworks.SelectMany(x => x.Providers).Distinct().ToList();
    }
}