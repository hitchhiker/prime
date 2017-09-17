using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Utility;

namespace Prime.Core
{
    public class AssetExchangeData : ModelBase
    {
        private AssetExchangeData() { }

        public AssetExchangeData(AssetPair pair, INetworkProvider provider)
        {
            ProviderId = provider.Id;
            AssetPair = pair;
        }

        public void Refresh(IDataContext context)
        {
            if (IsFresh)
                return;

            if (Provider != null)
            {
                var r = ApiCoordinator.RefreshCoinInfo(Provider, this);
                if (!r.IsNull && r.Response)
                    this.Save(context);
            }
        }

        private IAssetPairAggregationProvider _provider;
        public IAssetPairAggregationProvider Provider
        {
            get { return _provider ?? (_provider = Networks.I.AssetPairAggregationProviders.FirstOrDefault(x => x.Id == ProviderId)); }
        }

        public bool IsFresh => UtcUpdated.IsWithinTheLast(TimeSpan.FromDays(3));

        [Bson]
        public DateTime UtcUpdated { get; set; }

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