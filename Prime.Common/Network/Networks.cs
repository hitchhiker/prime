using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public class Networks : UniqueList<Network>
    {
        private Networks()
        {
            base.AddRange(TypeCatalogue.I.ImplementInstances<INetworkProvider>().Select(x=>x.Network).ToUniqueList());
        }

        public static Networks I => Lazy.Value;
        private static readonly Lazy<Networks> Lazy = new Lazy<Networks>(()=>new Networks());

        private IReadOnlyList<INetworkProvider> _providers;
        public IReadOnlyList<INetworkProvider> Providers => _providers ?? (_providers = TypeCatalogue.I.ImplementInstances<INetworkProvider>().Where(x=>!x.Disabled).OrderByDescending(x=>x.Priority).ToList());

        private IReadOnlyList<IExchangeProvider> _eProviders;
        public IReadOnlyList<IExchangeProvider> ExchangeProviders => _eProviders ?? (_eProviders = Providers.OfList<IExchangeProvider>());

        private IReadOnlyList<IWalletService> _wProviders;
        public IReadOnlyList<IWalletService> WalletProviders => _wProviders ?? (_wProviders = Providers.OfList<IWalletService>());

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
    }
}