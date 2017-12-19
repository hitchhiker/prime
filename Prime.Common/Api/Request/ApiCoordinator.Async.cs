using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Utility;

namespace Prime.Common
{
    public static partial class ApiCoordinator
    {
        public static Task<ApiResponse<MarketPrices>> GetPricingAsync(IPublicPricingProvider provider, PublicPricesContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.GetPricingAsync(context), nameof(GetPricing), provider, context);
        }

        public static Task<ApiResponse<bool>> TestApiAsync(INetworkProviderPrivate provider, ApiPrivateTestContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.TestPrivateApiAsync(context), nameof(TestApi), provider, context);
        }

        public static Task<ApiResponse<AssetPairs>> GetAssetPairsAsync(IAssetPairsProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();

            return AssetPairCache.I.TryAsync(provider, () => ApiHelpers.WrapExceptionAsync(() => provider.GetAssetPairsAsync(context), nameof(GetAssetPairs), provider, context));
        }

        public static Task<ApiResponse<OhlcData>> GetOhlcAsync(IOhlcProvider provider, OhlcContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.GetOhlcAsync(context), nameof(GetOhlc), provider, context);
        }

        public static Task<ApiResponse<TransferSuspensions>> GetTransferSuspensionsAsync(IDepositProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();
            return ApiHelpers.WrapExceptionAsync(() => provider.GetTransferSuspensionsAsync(context), nameof(GetTransferSuspensions), provider, context);
        }

        public static Task<ApiResponse<WalletAddresses>> GetDepositAddressesAsync(IDepositProvider provider, WalletAddressAssetContext context)
        {
            // TODO: review.
            //if (provider.CanGenerateDepositAddress && !provider.CanPeekDepositAddress)
            //    throw new Exception($"{provider.Title} cannot 'peek' deposit addresses.");

            return ApiHelpers.WrapExceptionAsync(() => provider.GetAddressesForAssetAsync(context), nameof(GetDepositAddresses), provider, context);
        }

        public static Task<ApiResponse<WalletAddresses>> GetAllDepositAddressesAsync(IDepositProvider provider, WalletAddressContext context)
        {
            // TODO: review
            //if (provider.CanGenerateDepositAddress && !provider.CanPeekDepositAddress)
            //    throw new Exception($"{provider.Title} cannot 'peek' deposit addresses.");

            return ApiHelpers.WrapExceptionAsync(() => provider.GetAddressesAsync(context), nameof(GetDepositAddresses), provider, context);
        }

        private static async Task<BalanceResults> CheckedBalancesAsync(IBalanceProvider provider, NetworkProviderPrivateContext context)
        {
            var r = await provider.GetBalancesAsync(context).ConfigureAwait(false);
            if (r == null)
                return null;

            r.RemoveAll(x => x.AvailableAndReserved == 0 && x.Available == 0 && x.Reserved == 0);
            return r;
        }

        public static Task<ApiResponse<BalanceResults>> GetBalancesAsync(IBalanceProvider provider, NetworkProviderPrivateContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => CheckedBalancesAsync(provider,context), nameof(GetBalances), provider, context);
        }

        public static Task<ApiResponse<List<AssetInfo>>> GetCoinInformationAsync(ICoinInformationProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();
            return ApiHelpers.WrapExceptionAsync(() => provider.GetCoinInformationAsync(context), nameof(GetCoinInformation), provider, context);
        }

        public static Task<ApiResponse<AggregatedAssetPairData>> GetCoinSnapshotAsync(ICoinSnapshotAggregationProvider provider, AssetPairDataContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.GetCoinSnapshotAsync(context), nameof(GetCoinSnapshot), provider, context);
        }

        public static Task<ApiResponse<PublicVolumeResponse>> GetAggVolumeDataAsync(IAggVolumeDataProvider provider, AggVolumeDataContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.GetAggVolumeDataAsync(context), nameof(GetAggVolumeData), provider, context);
        }

        public static Task<ApiResponse<PublicVolumeResponse>> GetPublicVolumeAsync(IPublicVolumeProvider provider, PublicVolumesContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.GetPublicVolumeAsync(context), nameof(GetPublicVolume), provider, context, r =>
            {
                var s = new VolumeSource(provider, typeof(IPublicVolumeProvider));
                r.Volume.ForEach(x => x.Source = s);
            });
        }

        public static Task<ApiResponse<OrderBook>> GetOrderBookAsync(IOrderBookProvider provider, OrderBookContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.GetOrderBookAsync(context), nameof(GetOrderBook), provider, context);
        }
    }
}