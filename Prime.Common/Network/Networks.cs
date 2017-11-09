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

        private readonly ConcurrentDictionary<ObjectId, Network> _cache = new ConcurrentDictionary<ObjectId, Network>();

        public Network Get(string name)
        {
            lock (_lock)
            {
                if (!_collected)
                {
                    _collected = true;
                    TypeCatalogue.I.ImplementInstances<INetworkProvider>().Select(x => x.Network).ToList();
                }
            }
            return _cache.GetOrAdd(Network.GetHash(name), k => new Network(name));
        }

        private bool _collected;
        private IReadOnlyList<INetworkProvider> _providers;
        public IReadOnlyList<INetworkProvider> Providers => _providers ?? (_providers = TypeCatalogue.I.ImplementInstances<INetworkProvider>().Where(x=>!x.Disabled).OrderByDescending(x=>x.Priority).ToList());

        private IReadOnlyList<IExchangeProvider> _eProviders;
        public IReadOnlyList<IExchangeProvider> ExchangeProviders => _eProviders ?? (_eProviders = Providers.OfList<IExchangeProvider>());

        private IReadOnlyList<IBalanceProvider> _wProviders;
        public IReadOnlyList<IBalanceProvider> WalletProviders => _wProviders ?? (_wProviders = Providers.OfList<IBalanceProvider>());

        private IReadOnlyList<IAssetPairsProvider> _assetPairProviders;
        public IReadOnlyList<IAssetPairsProvider> AssetPairsProviders => _assetPairProviders ?? (_assetPairProviders = Providers.OfList<IAssetPairsProvider>());

        private IReadOnlyList<IPublicPriceProvider> _ppProviders;
        public IReadOnlyList<IPublicPriceProvider> PublicPriceProviders => _ppProviders ?? (_ppProviders = Providers.OfList<IPublicPriceProvider>());

        private IReadOnlyList<ICoinInformationProvider> _coinListProviders;
        public IReadOnlyList<ICoinInformationProvider> CoinListProviders => _coinListProviders ?? (_coinListProviders = Providers.OfList<ICoinInformationProvider>());

        private IReadOnlyList<IOhlcProvider> _ohlcProviders;
        public IReadOnlyList<IOhlcProvider> OhlcProviders => _ohlcProviders ?? (_ohlcProviders = Providers.OfList<IOhlcProvider>());

        private IReadOnlyList<IAssetPairAggregationProvider> _apaggProviders;
        public IReadOnlyList<IAssetPairAggregationProvider> AssetPairAggregationProviders => _apaggProviders ?? (_apaggProviders = Providers.OfList<IAssetPairAggregationProvider>());

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