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
            return Task.FromResult(default(ApiResponse<MarketPrice>));
        }

        public static Task<ApiResponse<MarketPricesResult>> GetPricesAsync(IPublicPriceSuper provider, PublicPricesContext context)
        {
            switch (provider)
            {
                case IPublicPricesProvider ips:
                    return GetPricesAsync(ips, context);
                case IPublicPriceProvider ip:
                    var r = new MarketPricesResult();
                    foreach (var i in context.Pairs)
                    {
                        var rq = GetPrice(ip, new PublicPriceContext(i));
                        if (!rq.IsNull)
                            r.MarketPrices.Add(rq.Response);
                    }
                    return Task.FromResult(new ApiResponse<MarketPricesResult>(r));
            }
            return Task.FromResult(default(ApiResponse<MarketPricesResult>));
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

        public static Task<ApiResponse<MarketPrice>> GetPriceAsync(IPublicPriceProvider provider, PublicPriceContext context)
        {
            return ApiHelpers.WrapExceptionAsync(()=> provider.GetPriceAsync(context), nameof(GetPrice), provider, context);
        }

        public static Task<ApiResponse<MarketPrice>> GetPriceAsync(IPublicAssetPricesProvider provider, PublicPriceContext context)
        {
            return ApiHelpers.WrapExceptionAsync(async delegate
            {
                var r = await provider.GetAssetPricesAsync(context).ConfigureAwait(false);
                return r.MarketPrices.FirstOrDefault();
            }, "GetPrices (x1)", provider, context);
        }

        public static Task<ApiResponse<MarketPricesResult>> GetPricesAsync(IPublicPricesProvider provider, PublicPricesContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.GetPricesAsync(context), nameof(GetPrices), provider, context);
        }

        public static Task<ApiResponse<MarketPricesResult>> GetAssetPricesAsync(IPublicAssetPricesProvider provider, PublicAssetPricesContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.GetAssetPricesAsync(context), nameof(GetAssetPrices), provider, context);
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

        public static Task<ApiResponse<VolumeDataExchanges>> GetAggVolumeDataAsync(IAggVolumeDataProvider provider, AggVolumeDataContext context)
        {
            return ApiHelpers.WrapExceptionAsync(() => provider.GetAggVolumeDataAsync(context), nameof(GetAggVolumeData), provider, context);
        }
    }
}