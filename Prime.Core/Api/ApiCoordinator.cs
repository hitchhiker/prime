using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Prime.Core
{
    public static class ApiCoordinator
    {
        public static async Task<ApiResponse<T>> WrapException<T>(Func<Task<T>> t, NetworkProviderContext context)
        {
            if (t == null)
                return new ApiResponse<T>("Not implemented");

            try
            {
                var response = await t.Invoke();
                return new ApiResponse<T>(response);
            }
            catch (ApiResponseException ae)
            {
                return new ApiResponse<T>(ae.Message);
            }
            catch (Exception e)
            {
                return new ApiResponse<T>(e);
            }
        }

        public static Task<ApiResponse<AssetPairs>> GetAssetPairsAsync(IExchangeProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();
            return WrapException(()=> provider.GetAssetPairs(context), context);
        }

        public static ApiResponse<AssetPairs> GetAssetPairs(IExchangeProvider provider, NetworkProviderContext context = null)
        {
            return AsyncContext.Run(() => GetAssetPairsAsync(provider, context));
        }

        public static Task<ApiResponse<LatestPrice>> GetLatestPriceAsync(IPublicPriceProvider provider, PublicPriceContext context)
        {
            return WrapException(()=> provider.GetLatestPriceAsync(context), context);
        }

        public static ApiResponse<LatestPrice> GetLatestPrice(IPublicPriceProvider provider, PublicPriceContext context)
        {
            return AsyncContext.Run(() => GetLatestPriceAsync(provider, context));
        }

        public static Task<ApiResponse<LatestPrices>> GetLatestPricesAsync(IPublicPricesProvider provider, PublicPricesContext context)
        {
            return WrapException(()=> provider.GetLatestPricesAsync(context), context);
        }

        public static ApiResponse<LatestPrices> GetLatestPrices(IPublicPricesProvider provider, PublicPricesContext context)
        {
            return AsyncContext.Run(() => GetLatestPricesAsync(provider, context));
        }

        public static Task<ApiResponse<List<AssetInfo>>> GetCoinInfoAsync(ICoinInformationProvider provider, NetworkProviderContext context = null)
        {
            context = context ?? new NetworkProviderContext();
            return WrapException(() => provider.GetCoinInfoAsync(context), context);
        }

        public static ApiResponse<List<AssetInfo>> GetCoinInfo(ICoinInformationProvider provider, NetworkProviderContext context = null)
        {
            return AsyncContext.Run(() => GetCoinInfoAsync(provider, context));
        }

        public static Task<ApiResponse<OhclData>> GetOhlcAsync(IOhlcProvider provider, OhlcContext context)
        {
            return WrapException(() => provider.GetOhlcAsync(context), context);
        }

        public static ApiResponse<OhclData> GetOhlc(IOhlcProvider provider, OhlcContext context)
        {
            return AsyncContext.Run(() => GetOhlcAsync(provider, context));
        }

        public static Task<ApiResponse<WalletAddresses>> FetchDepositAddressesAsync(IWalletService provider, WalletAddressAssetContext context)
        {
            return WrapException(() => provider.FetchDepositAddressesAsync(context), context);
        }

        public static ApiResponse<WalletAddresses> FetchDepositAddresses(IWalletService provider, WalletAddressAssetContext context)
        {
            return AsyncContext.Run(() => FetchDepositAddressesAsync(provider, context));
        }

        public static Task<ApiResponse<WalletAddresses>> FetchAllDepositAddressesAsync(IWalletService provider, WalletAddressContext context)
        {
            return WrapException(() => provider.FetchAllDepositAddressesAsync(context), context);
        }

        public static ApiResponse<WalletAddresses> FetchAllDepositAddresses(IWalletService provider, WalletAddressContext context)
        {
            return AsyncContext.Run(() => FetchAllDepositAddressesAsync(provider, context));
        }

        public static Task<ApiResponse<BalanceResults>> GetBalancesAsync(IWalletService provider, NetworkProviderPrivateContext context)
        {
            return WrapException(() => provider.GetBalancesAsync(context), context);
        }

        public static ApiResponse<BalanceResults> GetBalances(IWalletService provider, NetworkProviderPrivateContext context)
        {
            return AsyncContext.Run(() => GetBalancesAsync(provider, context));
        }

        public static Task<ApiResponse<bool>> TestApiAsync(INetworkProviderPrivate provider, ApiTestContext context)
        {
            return WrapException(() => provider.TestApiAsync(context), context);
        }

        public static ApiResponse<bool> TestApi(INetworkProviderPrivate provider, ApiTestContext context)
        {
            return AsyncContext.Run(() => TestApiAsync(provider, context));
        }
    }
}
