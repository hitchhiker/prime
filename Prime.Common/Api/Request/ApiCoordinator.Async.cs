using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Prime.Common
{
    public static partial class ApiCoordinator
    {
        public static Task<ApiResponse<MarketPrice>> GetPriceAsync(IPublicPriceSuper provider, PublicPriceContext context)
        {
            switch (provider)
            {
                case IPublicPriceProvider ip:
                    return GetPriceAsync(ip, context);
                case IPublicAssetPricesProvider ips:
                    return GetPriceAsync(ips, context);
            }
            return null;
        }

        public static Task<ApiResponse<bool>> TestApiAsync(INetworkProviderPrivate provider, ApiTestContext context)
        {
            return ApiHelpers.WrapException(() => provider.TestApiAsync(context), nameof(TestApi), provider, context);
        }

        public static Task<ApiResponse<AssetPairs>> GetAssetPairsAsync(IAssetPairsProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();

            return AssetPairCache.I.TryAsync(provider,
                async () => await ApiHelpers.WrapException(() => provider.GetAssetPairsAsync(context), nameof(GetAssetPairs),
                    provider, context));
        }

        public static Task<ApiResponse<MarketPrice>> GetPriceAsync(IPublicPriceProvider provider, PublicPriceContext context)
        {
            return ApiHelpers.WrapException(()=> provider.GetPriceAsync(context), nameof(GetPrice), provider, context);
        }

        public static Task<ApiResponse<MarketPrice>> GetPriceAsync(IPublicAssetPricesProvider provider, PublicPriceContext context)
        {
            return ApiHelpers.WrapException(async delegate
            {
                var r = await provider.GetAssetPricesAsync(context);
                return r.MarketPrices.FirstOrDefault();
            }, "GetPrices (x1)", provider, context);
        }

        public static Task<ApiResponse<MarketPricesResult>> GetPricesAsync(IPublicPricesProvider provider, PublicPricesContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetPricesAsync(context), nameof(GetPrices), provider, context);
        }

        public static Task<ApiResponse<MarketPricesResult>> GetAssetPricesAsync(IPublicAssetPricesProvider provider, PublicAssetPricesContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetAssetPricesAsync(context), nameof(GetAssetPrices), provider, context);
        }
        
        public static Task<ApiResponse<OhlcData>> GetOhlcAsync(IOhlcProvider provider, OhlcContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetOhlcAsync(context), nameof(GetOhlc), provider, context);
        }

        public static Task<ApiResponse<WalletAddresses>> GetDepositAddressesAsync(IDepositProvider provider, WalletAddressAssetContext context)
        {
            // TODO: review.
            //if (provider.CanGenerateDepositAddress && !provider.CanPeekDepositAddress)
            //    throw new Exception($"{provider.Title} cannot 'peek' deposit addresses.");

            return ApiHelpers.WrapException(() => provider.GetAddressesForAssetAsync(context), nameof(GetDepositAddresses), provider, context);
        }

        public static Task<ApiResponse<WalletAddresses>> GetAllDepositAddressesAsync(IDepositProvider provider, WalletAddressContext context)
        {
            // TODO: review
            //if (provider.CanGenerateDepositAddress && !provider.CanPeekDepositAddress)
            //    throw new Exception($"{provider.Title} cannot 'peek' deposit addresses.");

            return ApiHelpers.WrapException(() => provider.GetAddressesAsync(context), nameof(GetDepositAddresses), provider, context);
        }

        private static async Task<BalanceResults> CheckedBalancesAsync(IBalanceProvider provider, NetworkProviderPrivateContext context)
        {
            var r = await provider.GetBalancesAsync(context);
            if (r == null)
                return null;

            r.RemoveAll(x => x.Balance == 0 && x.Available == 0 && x.Reserved == 0);
            return r;
        }

        public static Task<ApiResponse<BalanceResults>> GetBalancesAsync(IBalanceProvider provider, NetworkProviderPrivateContext context)
        {
            return ApiHelpers.WrapException(() => CheckedBalancesAsync(provider,context), nameof(GetBalances), provider, context);
        }

        public static Task<ApiResponse<List<AssetInfo>>> GetCoinInformationAsync(ICoinInformationProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();
            return ApiHelpers.WrapException(() => provider.GetCoinInformationAsync(context), nameof(GetCoinInformation), provider, context);
        }

        public static Task<ApiResponse<AggregatedAssetPairData>> GetCoinSnapshotAsync(IAssetPairAggregationProvider provider, AssetPairDataContext context)
        {
            return ApiHelpers.WrapException(() => provider.GetCoinSnapshotAsync(context), nameof(GetCoinSnapshot), provider, context);
        }
    }
}