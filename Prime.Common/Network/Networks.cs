using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public sealed class Networks : IEnumerable<Network>
    {
        public static Networks I => Lazy.Value;
        private static readonly Lazy<Networks> Lazy = new Lazy<Networks>(()=>new Networks());
        private readonly object _lock = new object();
        public readonly UniqueList<Network> KnownNetworks = new UniqueList<Network>();

        private readonly ConcurrentDictionary<ObjectId, Network> _cache = new ConcurrentDictionary<ObjectId, Network>();

        public Network Get(string name)
        {
            lock (_lock)
            {
                if (!_collected)
                {
                    _collected = true;
                    KnownNetworks.AddRange(TypeCatalogue.I.ImplementInstances<INetworkProvider>().Where(x=>!x.Disabled).Select(x => x.Network));
                }
            }
            return _cache.GetOrAdd(Network.GetHash(name), k => new Network(name));
        }

        private bool _collected;
        private IReadOnlyList<INetworkProvider> _providers;
        public IReadOnlyList<INetworkProvider> Providers => _providers ?? (_providers = TypeCatalogue.I.ImplementInstances<INetworkProvider>().Where(x=>!x.Disabled).OrderByDescending(x=>x.Priority).ToList());

        private IReadOnlyList<IBalanceProvider> _balanceProviders;
        public IReadOnlyList<IBalanceProvider> BalanceProviders => _balanceProviders ?? (_balanceProviders = Providers.OfList<IBalanceProvider>());

        private IReadOnlyList<IDepositProvider> _depositProviders;
        public IReadOnlyList<IDepositProvider> DepositProviders => _depositProviders ?? (_depositProviders = Providers.OfList<IDepositProvider>());

        private IReadOnlyList<IAssetPairsProvider> _assetPairProviders;
        public IReadOnlyList<IAssetPairsProvider> AssetPairsProviders => _assetPairProviders ?? (_assetPairProviders = Providers.OfList<IAssetPairsProvider>());

        private IReadOnlyList<IPublicPricingProvider> _ppProviders;
        public IReadOnlyList<IPublicPricingProvider> PublicPricingProviders => _ppProviders ?? (_ppProviders = Providers.OfList<IPublicPricingProvider>());

        private IReadOnlyList<ICoinInformationProvider> _coinListProviders;
        public IReadOnlyList<ICoinInformationProvider> CoinListProviders => _coinListProviders ?? (_coinListProviders = Providers.OfList<ICoinInformationProvider>());

        private IReadOnlyList<IOhlcProvider> _ohlcProviders;
        public IReadOnlyList<IOhlcProvider> OhlcProviders => _ohlcProviders ?? (_ohlcProviders = Providers.OfList<IOhlcProvider>());

        private IReadOnlyList<ICoinSnapshotAggregationProvider> _apaggProviders;
        public IReadOnlyList<ICoinSnapshotAggregationProvider> AssetPairAggregationProviders => _apaggProviders ?? (_apaggProviders = Providers.OfList<ICoinSnapshotAggregationProvider>());

        public IEnumerator<Network> GetEnumerator()
        {
            return _cache.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_cache.Values).GetEnumerator();
        }
    }
}