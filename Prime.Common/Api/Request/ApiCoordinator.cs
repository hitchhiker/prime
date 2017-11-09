using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Nito.AsyncEx;
using Prime.Utility;

namespace Prime.Common
{
    public static partial class ApiCoordinator
    {
        public static ApiResponse<MarketPrice> GetPrice(IPublicPriceSuper provider, PublicPriceContext context)
        {
            return AsyncContext.Run(() => GetPriceAsync(provider, context));
        }

        public static ApiResponse<bool> TestApi(INetworkProviderPrivate provider, ApiPrivateTestContext context)
        {
            return AsyncContext.Run(() => TestApiAsync(provider, context));
        }

        public static ApiResponse<MarketPrice> GetPrice(IPublicPriceProvider provider, PublicPriceContext context)
        {
            return AsyncContext.Run(() => GetPriceAsync(provider, context));
        }

        public static ApiResponse<AssetPairs> GetAssetPairs(IAssetPairsProvider provider, NetworkProviderContext context = null)
        {
            return AsyncContext.Run(() => GetAssetPairsAsync(provider, context));
        }

        public static ApiResponse<MarketPrice> GetPrice(IPublicAssetPricesProvider provider, PublicPriceContext context)
        {
            return AsyncContext.Run(() => GetPriceAsync(provider, context));
        }

        public static ApiResponse<MarketPricesResult> GetAssetPrices(IPublicAssetPricesProvider provider, PublicAssetPricesContext context)
        {
            return AsyncContext.Run(() => GetAssetPricesAsync(provider, context));
        }

        public static ApiResponse<MarketPricesResult> GetPrices(IPublicPricesProvider provider, PublicPricesContext context)
        {
            return AsyncContext.Run(() => GetPricesAsync(provider, context));
        }
        
        public static ApiResponse<OhlcData> GetOhlc(IOhlcProvider provider, OhlcContext context)
        {
            return AsyncContext.Run(() => GetOhlcAsync(provider, context));
        }

        public static ApiResponse<WalletAddresses> GetDepositAddresses(IDepositProvider provider, WalletAddressAssetContext context)
        {
            return AsyncContext.Run(() => GetDepositAddressesAsync(provider, context));
        }

        public static ApiResponse<WalletAddresses> FetchAllDepositAddresses(IDepositProvider provider, WalletAddressContext context)
        {
            return AsyncContext.Run(() => GetAllDepositAddressesAsync(provider, context));
        }

        public static ApiResponse<BalanceResults> GetBalances(IBalanceProvider provider, NetworkProviderPrivateContext context)
        {
            return AsyncContext.Run(() => GetBalancesAsync(provider, context));
        }

        public static ApiResponse<List<AssetInfo>> GetCoinInformation(ICoinInformationProvider provider, NetworkProviderContext context = null)
        {
            return AsyncContext.Run(() => GetCoinInformationAsync(provider, context));
        }

        public static ApiResponse<AggregatedAssetPairData> GetCoinSnapshot(IAssetPairAggregationProvider provider, AssetPairDataContext context)
        {
            return AsyncContext.Run(() => GetCoinSnapshotAsync(provider, context));
        }
    }
}
