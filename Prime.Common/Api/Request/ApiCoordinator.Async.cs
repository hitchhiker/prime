using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Prime.Common
{
    public static partial class ApiCoordinator
    {
        public static async Task<ApiResponse<MarketPricesResult>> GetPricingAsync(IPublicPricingProvider provider, PublicPricesContext context)
        {
            //if (context.IsMultiple && context.ForSingleMethod)
            //    throw new ArgumentException($"Failed as '{nameof(context.ForSingleMethod)}' is set on '{context.GetType().Name}' but this context requires '{nameof(context.IsMultiple)}'");

            var features = provider.PricingFeatures;

            if (!features.HasBulk && !features.HasSingle)
                throw new Exception($"Fatal: The provider {provider.Title} supports neither {nameof(features.HasBulk)} nor {nameof(features.HasSingle)}.");

            var tryBulk = context.IsMultiple && !context.ForSingleMethod;

            var channel = tryBulk ? features.Bulk : (PricingFeaturesItemBase)features.Single;

            channel = channel ?? (tryBulk ? features.Single : (PricingFeaturesItemBase)features.Bulk);

            if (context.RequestStatistics && !channel.CanSatistics)
                throw new ArgumentException($"This provider {provider.Title} cannot provide statistics for '{channel.GetType().Name}'");

            if (context.RequestVolume && !channel.CanVolume)
                throw new ArgumentException($"This provider {provider.Title} cannot provide volume data for '{channel.GetType().Name}'");
            
            switch (channel)
            {
                case PricingBulkFeatures bulk:
                    return await ApiHelpers.WrapExceptionAsync(() => provider.GetPricingAsync(context), nameof(GetPricing) + " [Bulk]", provider, context).ConfigureAwait(false);
                case PricingSingleFeatures single:
                    var r = new MarketPricesResult();
                    foreach (var pair in context.Pairs)
                    {
                        var ctx = new PublicPriceContext(pair);
                        var rq = await ApiHelpers.WrapExceptionAsync(() => provider.GetPricingAsync(ctx), nameof(GetPricing) + " [Bulk Sim]", provider, context).ConfigureAwait(false);
                        if (!rq.IsNull && rq.Response.FirstPrice != null)
                            r.MarketPrices.Add(rq.Response.FirstPrice);
                        else
                            r.MissedPairs.Add(pair);
                    }
                    return new ApiResponse<MarketPricesResult>(r);
            }

            return default;
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

            r.RemoveAll(x => x.Balance == 0 && x.Available == 0 && x.Reserved == 0);
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

        public static Task<ApiResponse<NetworkPairVolumeData>> GetAggVolumeDataAsync(IAggVolumeDataProvider provider, AggVolumeDataContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.GetAggVolumeDataAsync(context), nameof(GetAggVolumeData), provider, context);
        }

        public static Task<ApiResponse<NetworkPairVolume>> GetAssetPairVolumeAsync(IAssetPairVolumeProvider provider, VolumeContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.GetAssetPairVolumeAsync(context), nameof(GetAssetPairVolume), provider, context);
        }
    }
}