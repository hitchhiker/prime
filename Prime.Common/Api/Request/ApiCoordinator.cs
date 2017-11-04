﻿using System;
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
        public static ApiResponse<LatestPrice> GetPrice(IPublicPriceSuper provider, PublicPriceContext context)
        {
            return AsyncContext.Run(() => GetPriceAsync(provider, context));
        }

        public static ApiResponse<bool> TestApi(INetworkProviderPrivate provider, ApiTestContext context)
        {
            return AsyncContext.Run(() => TestApiAsync(provider, context));
        }

        public static ApiResponse<LatestPrice> GetPrice(IPublicPriceProvider provider, PublicPriceContext context)
        {
            return AsyncContext.Run(() => GetPriceAsync(provider, context));
        }

        public static ApiResponse<AssetPairs> GetAssetPairs(IAssetPairsProvider provider, NetworkProviderContext context = null)
        {
            return AsyncContext.Run(() => GetAssetPairsAsync(provider, context));
        }

        public static ApiResponse<LatestPrice> GetPrice(IPublicAssetPricesProvider provider, PublicPriceContext context)
        {
            return AsyncContext.Run(() => GetPriceAsync(provider, context));
        }

        public static ApiResponse<List<LatestPrice>> GetAssetPrices(IPublicAssetPricesProvider provider, PublicAssetPricesContext context)
        {
            return AsyncContext.Run(() => GetAssetPricesAsync(provider, context));
        }

        public static ApiResponse<List<LatestPrice>> GetPrices(IPublicPricesProvider provider, PublicPricesContext context)
        {
            return AsyncContext.Run(() => GetPricesAsync(provider, context));
        }
        
        public static ApiResponse<OhlcData> GetOhlc(IOhlcProvider provider, OhlcContext context)
        {
            return AsyncContext.Run(() => GetOhlcAsync(provider, context));
        }

        public static ApiResponse<WalletAddresses> GetDepositAddresses(IWalletService provider, WalletAddressAssetContext context)
        {
            return AsyncContext.Run(() => GetDepositAddressesAsync(provider, context));
        }

        public static ApiResponse<WalletAddresses> FetchAllDepositAddresses(IWalletService provider, WalletAddressContext context)
        {
            return AsyncContext.Run(() => GetAllDepositAddressesAsync(provider, context));
        }

        public static ApiResponse<BalanceResults> GetBalances(IWalletService provider, NetworkProviderPrivateContext context)
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
