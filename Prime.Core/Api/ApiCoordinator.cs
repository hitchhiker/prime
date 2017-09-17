using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Utility;

namespace Prime.Core
{
    public static class ApiCoordinator
    {
        public static Task<ApiResponse<bool>> TestApiAsync(INetworkProviderPrivate provider, ApiTestContext context)
        {
            return ApiHelpers.WrapException(() => provider.TestApiAsync(context), "TestApi", provider, context);
        }

        public static ApiResponse<bool> TestApi(INetworkProviderPrivate provider, ApiTestContext context)
        {
            return AsyncContext.Run(() => TestApiAsync(provider, context));
        }

        public static Task<ApiResponse<AssetPairs>> GetAssetPairsAsync(IExchangeProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();
            return ApiHelpers.WrapException(()=> provider.GetAssetPairs(context), "GetAssetPairs", provider, context);
        }

        public static ApiResponse<AssetPairs> GetAssetPairs(IExchangeProvider provider, NetworkProviderContext context = null)
        {
            return AsyncContext.Run(() => GetAssetPairsAsync(provider, context));
        }

        public static Task<ApiResponse<LatestPrice>> GetLatestPriceAsync(IPublicPriceProvider provider, PublicPriceContext context)
        {
            return ApiHelpers.WrapException(()=> provider.GetLatestPriceAsync(context), "GetLatestPrice", provider, context);
        }

        public static ApiResponse<LatestPrice> GetLatestPrice(IPublicPriceProvider provider, PublicPriceContext context)
        {
            return AsyncContext.Run(() => GetLatestPriceAsync(provider, context));
        }

        public static Task<ApiResponse<LatestPrices>> GetLatestPricesAsync(IPublicPricesProvider provider, PublicPricesContext context)
        {
            return ApiHelpers.WrapException(()=> provider.GetLatestPricesAsync(context), "GetLatestPrices", provider, context);
        }

        public static ApiResponse<LatestPrices> GetLatestPrices(IPublicPricesProvider provider, PublicPricesContext context)
        {
            return AsyncContext.Run(() => GetLatestPricesAsync(provider, context));
        }

        public static Task<ApiResponse<List<AssetInfo>>> GetCoinInfoAsync(ICoinInformationProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();
            return ApiHelpers.WrapException(() => provider.GetCoinInfoAsync(context), "GetCoinInfo", provider, context);
        }

        public static ApiResponse<List<AssetInfo>> GetCoinInfo(ICoinInformationProvider provider, NetworkProviderContext context = null)
        {
            return AsyncContext.Run(() => GetCoinInfoAsync(provider, context));
        }

        public static Task<ApiResponse<OhclData>> GetOhlcAsync(IOhlcProvider provider, OhlcContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetOhlcAsync(context), "GetOhlc", provider, context);
        }

        public static ApiResponse<OhclData> GetOhlc(IOhlcProvider provider, OhlcContext context)
        {
            return AsyncContext.Run(() => GetOhlcAsync(provider, context));
        }

        public static Task<ApiResponse<WalletAddresses>> GetDepositAddressesAsync(IWalletService provider, WalletAddressAssetContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetDepositAddressesAsync(context), "GetDepositAddresses", provider, context);
        }

        public static ApiResponse<WalletAddresses> GetDepositAddresses(IWalletService provider, WalletAddressAssetContext context)
        {
            return AsyncContext.Run(() => GetDepositAddressesAsync(provider, context));
        }

        public static Task<ApiResponse<WalletAddresses>> GetAllDepositAddressesAsync(IWalletService provider, WalletAddressContext context)
        {
            return ApiHelpers.WrapException(() => provider.FetchAllDepositAddressesAsync(context), "GetDepositAddresses", provider, context);
        }

        public static ApiResponse<WalletAddresses> FetchAllDepositAddresses(IWalletService provider, WalletAddressContext context)
        {
            return AsyncContext.Run(() => GetAllDepositAddressesAsync(provider, context));
        }

        public static Task<ApiResponse<BalanceResults>> GetBalancesAsync(IWalletService provider, NetworkProviderPrivateContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetBalancesAsync(context), "GetBalances", provider, context);
        }

        public static ApiResponse<BalanceResults> GetBalances(IWalletService provider, NetworkProviderPrivateContext context)
        {
            return AsyncContext.Run(() => GetBalancesAsync(provider, context));
        }

        public static Task<ApiResponse<AssetExchangeData>> GetCoinInfoAsync(IAssetPairAggregationProvider provider, AggregatedCoinInfoContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetCoinInfoAsync(context), "GetCoinInfo", provider, context);
        }

        public static ApiResponse<AssetExchangeData> GetCoinInfo(IAssetPairAggregationProvider provider, AggregatedCoinInfoContext context)
        {
            return AsyncContext.Run(() => GetCoinInfoAsync(provider, context));
        }

        public static Task<ApiResponse<bool>> RefreshCoinInfoAsync(IAssetPairAggregationProvider provider, AssetExchangeData context)
        {
            return ApiHelpers.WrapException(() => provider.RefreshCoinInfoAsync(context), "GetCoinInfo", provider, new NetworkProviderContext());
        }

        public static ApiResponse<bool> RefreshCoinInfo(IAssetPairAggregationProvider provider, AssetExchangeData context)
        {
            return AsyncContext.Run(() => RefreshCoinInfoAsync(provider, context));
        }
    }
}
