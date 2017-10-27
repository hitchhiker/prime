using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Utility;

namespace Prime.Common
{
    public static class ApiCoordinator
    {
        public static Task<ApiResponse<bool>> TestApiAsync(INetworkProviderPrivate provider, ApiTestContext context)
        {
            return ApiHelpers.WrapException(() => provider.TestApiAsync(context), nameof(TestApi), provider, context);
        }

        public static ApiResponse<bool> TestApi(INetworkProviderPrivate provider, ApiTestContext context)
        {
            return AsyncContext.Run(() => TestApiAsync(provider, context));
        }

        public static Task<ApiResponse<AssetPairs>> GetAssetPairsAsync(IAssetPairsProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();
            return ApiHelpers.WrapException(()=> provider.GetAssetPairs(context), nameof(GetAssetPairs), provider, context);
        }

        public static ApiResponse<AssetPairs> GetAssetPairs(IAssetPairsProvider provider, NetworkProviderContext context = null)
        {
            return AsyncContext.Run(() => GetAssetPairsAsync(provider, context));
        }

        public static Task<ApiResponse<LatestPrice>> GetPriceAsync(IPublicPriceProvider provider, PublicPriceContext context)
        {
            return ApiHelpers.WrapException(()=> provider.GetPriceAsync(context), nameof(GetPrice), provider, context);
        }

        public static ApiResponse<LatestPrice> GetPrice(IPublicPriceProvider provider, PublicPriceContext context)
        {
            return AsyncContext.Run(() => GetPriceAsync(provider, context));
        }

        public static Task<ApiResponse<LatestPrice>> GetPriceAsync(IPublicAssetPricesProvider provider, PublicPriceContext context)
        {
            return ApiHelpers.WrapException(async delegate
            {
                var r = await provider.GetAssetPricesAsync(context);
                return r.FirstOrDefault();
            }, "GetPrices (x1)", provider, context);
        }

        public static ApiResponse<LatestPrice> GetPrice(IPublicAssetPricesProvider provider, PublicPriceContext context)
        {
            return AsyncContext.Run(() => GetPriceAsync(provider, context));
        }

        public static Task<ApiResponse<List<LatestPrice>>> GetPricesAsync(IPublicPricesProvider provider, PublicPricesContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetPricesAsync(context), nameof(GetPrices), provider, context);
        }

        public static ApiResponse<List<LatestPrice>> GetPrices(IPublicPricesProvider provider, PublicPricesContext context)
        {
            return AsyncContext.Run(() => GetPricesAsync(provider, context));
        }

        public static Task<ApiResponse<List<LatestPrice>>> GetAssetPricesAsync(IPublicAssetPricesProvider provider, PublicAssetPricesContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetAssetPricesAsync(context), nameof(GetAssetPrices), provider, context);
        }

        public static ApiResponse<List<LatestPrice>> GetAssetPrices(IPublicAssetPricesProvider provider, PublicAssetPricesContext context)
        {
            return AsyncContext.Run(() => GetAssetPricesAsync(provider, context));
        }

        public static Task<ApiResponse<List<AssetInfo>>> GetSnapshotAsync(ICoinInformationProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();
            return ApiHelpers.WrapException(() => provider.GetCoinInfoAsync(context), nameof(GetCoinInfo), provider, context);
        }

        public static ApiResponse<List<AssetInfo>> GetCoinInfo(ICoinInformationProvider provider, NetworkProviderContext context = null)
        {
            return AsyncContext.Run(() => GetSnapshotAsync(provider, context));
        }

        public static Task<ApiResponse<OhlcData>> GetOhlcAsync(IOhlcProvider provider, OhlcContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetOhlcAsync(context), nameof(GetOhlc), provider, context);
        }

        public static ApiResponse<OhlcData> GetOhlc(IOhlcProvider provider, OhlcContext context)
        {
            return AsyncContext.Run(() => GetOhlcAsync(provider, context));
        }

        public static Task<ApiResponse<WalletAddresses>> GetDepositAddressesAsync(IWalletService provider, WalletAddressAssetContext context)
        {
            // TODO: review.
            //if (provider.CanGenerateDepositAddress && !provider.CanPeekDepositAddress)
            //    throw new Exception($"{provider.Title} cannot 'peek' deposit addresses.");

            return ApiHelpers.WrapException(() => provider.GetAddressesForAssetAsync(context), nameof(GetDepositAddresses), provider, context);
        }

        public static ApiResponse<WalletAddresses> GetDepositAddresses(IWalletService provider, WalletAddressAssetContext context)
        {
            return AsyncContext.Run(() => GetDepositAddressesAsync(provider, context));
        }

        public static Task<ApiResponse<WalletAddresses>> GetAllDepositAddressesAsync(IWalletService provider, WalletAddressContext context)
        {
            // TODO: review
            //if (provider.CanGenerateDepositAddress && !provider.CanPeekDepositAddress)
            //    throw new Exception($"{provider.Title} cannot 'peek' deposit addresses.");

            return ApiHelpers.WrapException(() => provider.GetAddressesAsync(context), nameof(GetDepositAddresses), provider, context);
        }

        public static ApiResponse<WalletAddresses> FetchAllDepositAddresses(IWalletService provider, WalletAddressContext context)
        {
            return AsyncContext.Run(() => GetAllDepositAddressesAsync(provider, context));
        }

        private static async Task<BalanceResults> CheckedBalancesAsync(IWalletService provider, NetworkProviderPrivateContext context)
        {
            var r = await provider.GetBalancesAsync(context);
            if (r == null)
                return null;

            r.RemoveAll(x => x.Balance == 0 && x.Available == 0 && x.Reserved == 0);
            return r;
        }

        public static Task<ApiResponse<BalanceResults>> GetBalancesAsync(IWalletService provider, NetworkProviderPrivateContext context)
        {
            return ApiHelpers.WrapException(() => CheckedBalancesAsync(provider,context), nameof(GetBalances), provider, context);
        }

        public static ApiResponse<BalanceResults> GetBalances(IWalletService provider, NetworkProviderPrivateContext context)
        {
            return AsyncContext.Run(() => GetBalancesAsync(provider, context));
        }

        public static Task<ApiResponse<AggregatedAssetPairData>> GetCoinSnapshotAsync(IAssetPairAggregationProvider provider, AssetPairDataContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetCoinSnapshotAsync(context), nameof(GetCoinSnapshot), provider, context);
        }

        public static ApiResponse<AggregatedAssetPairData> GetCoinSnapshot(IAssetPairAggregationProvider provider, AssetPairDataContext context)
        {
            return AsyncContext.Run(() => GetCoinSnapshotAsync(provider, context));
        }
    }
}
