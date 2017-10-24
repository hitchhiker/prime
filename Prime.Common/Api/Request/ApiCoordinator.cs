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
            return ApiHelpers.WrapException(() => provider.TestApiAsync(context), "TestApi", provider, context);
        }

        public static ApiResponse<bool> TestApi(INetworkProviderPrivate provider, ApiTestContext context)
        {
            return AsyncContext.Run(() => TestApiAsync(provider, context));
        }

        public static Task<ApiResponse<AssetPairs>> GetAssetPairsAsync(IExchangeProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();
            return ApiHelpers.WrapException(()=> provider.GetAssetPairs(context), nameof(GetAssetPairs), provider, context);
        }

        public static ApiResponse<AssetPairs> GetAssetPairs(IExchangeProvider provider, NetworkProviderContext context = null)
        {
            return AsyncContext.Run(() => GetAssetPairsAsync(provider, context));
        }

        public static Task<ApiResponse<LatestPrice>> GetPairPriceAsync(IPublicPairPriceProvider provider, PublicPairPriceContext context)
        {
            return ApiHelpers.WrapException(()=> provider.GetPairPriceAsync(context), "GetPairPrice", provider, context);
        }

        public static ApiResponse<LatestPrice> GetPairPrice(IPublicPairPriceProvider provider, PublicPairPriceContext context)
        {
            return AsyncContext.Run(() => GetPairPriceAsync(provider, context));
        }

        public static Task<ApiResponse<LatestPrices>> GetAssetPricesAsync(IPublicAssetPricesProvider provider, PublicAssetPricesContext context)
        {
            return ApiHelpers.WrapException(()=> provider.GetAssetPricesAsync(context), "GetAssetPrices", provider, context);
        }

        public static ApiResponse<LatestPrices> GetAssetPrices(IPublicAssetPricesProvider provider, PublicAssetPricesContext context)
        {
            return AsyncContext.Run(() => GetAssetPricesAsync(provider, context));
        }

        public static Task<ApiResponse<List<AssetInfo>>> GetSnapshotAsync(ICoinInformationProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();
            return ApiHelpers.WrapException(() => provider.GetCoinInfoAsync(context), "GetCoinInfo", provider, context);
        }

        public static ApiResponse<List<AssetInfo>> GetCoinInfo(ICoinInformationProvider provider, NetworkProviderContext context = null)
        {
            return AsyncContext.Run(() => GetSnapshotAsync(provider, context));
        }

        public static Task<ApiResponse<OhlcData>> GetOhlcAsync(IOhlcProvider provider, OhlcContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetOhlcAsync(context), "GetOhlc", provider, context);
        }

        public static ApiResponse<OhlcData> GetOhlc(IOhlcProvider provider, OhlcContext context)
        {
            return AsyncContext.Run(() => GetOhlcAsync(provider, context));
        }

        public static Task<ApiResponse<WalletAddresses>> GetDepositAddressesAsync(IWalletService provider, WalletAddressAssetContext context)
        {
            // TODO: implement.
            //if (context.CanGenerateAddress && !provider.CanPeekDepositAddress)
            //    throw new Exception($"{provider.Title} cannot 'peek' deposit addresses.");

            return ApiHelpers.WrapException(() => provider.GetAddressesForAssetAsync(context), "GetDepositAddresses", provider, context);
        }

        public static ApiResponse<WalletAddresses> GetDepositAddresses(IWalletService provider, WalletAddressAssetContext context)
        {
            return AsyncContext.Run(() => GetDepositAddressesAsync(provider, context));
        }

        public static Task<ApiResponse<WalletAddresses>> GetAllDepositAddressesAsync(IWalletService provider, WalletAddressContext context)
        {
            // TODO: implement.
            //if (context.CanGenerateAddress && !provider.CanPeekDepositAddress)
            //    throw new Exception($"{provider.Title} cannot 'peek' deposit addresses.");

            return ApiHelpers.WrapException(() => provider.GetAddressesAsync(context), "GetDepositAddresses", provider, context);
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
            return ApiHelpers.WrapException(() => CheckedBalancesAsync(provider,context), "GetBalances", provider, context);
        }

        public static ApiResponse<BalanceResults> GetBalances(IWalletService provider, NetworkProviderPrivateContext context)
        {
            return AsyncContext.Run(() => GetBalancesAsync(provider, context));
        }

        public static Task<ApiResponse<AggregatedAssetPairData>> GetCoinSnapshotAsync(IAssetPairAggregationProvider provider, AssetPairDataContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetCoinSnapshotAsync(context), "GetCoinSnapshot", provider, context);
        }

        public static ApiResponse<AggregatedAssetPairData> GetCoinSnapshot(IAssetPairAggregationProvider provider, AssetPairDataContext context)
        {
            return AsyncContext.Run(() => GetCoinSnapshotAsync(provider, context));
        }
    }
}
