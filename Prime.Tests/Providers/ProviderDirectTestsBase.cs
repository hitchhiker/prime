using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Common.Wallet.Withdrawal.Cancelation;
using Prime.Common.Wallet.Withdrawal.Confirmation;
using Prime.Common.Wallet.Withdrawal.History;
using Prime.Utility;

namespace Prime.Tests.Providers
{
    public abstract class ProviderDirectTestsBase
    {
        public INetworkProvider Provider { get; protected set; }

        #region Wrappers

        public virtual void TestApi()
        {
            var p = IsType<INetworkProviderPrivate>();
            if (p.Success)
                TestApi(p.Provider);
        }

        public virtual void TestPublicApi()
        {
            var p = IsType<INetworkProvider>();
            if (p.Success)
                TestPublicApi(p.Provider);
        }


        public virtual void TestGetOhlc() { }
        public void TestGetOhlc(OhlcContext context)
        {
            var p = IsType<IOhlcProvider>();
            if (p.Success)
                GetOhlcAsync(p.Provider, context);
        }

        public virtual void TestGetPricing() { }
        public void TestGetPricing(List<AssetPair> pairs, bool firstPriceLessThan1)
        {
            var p = IsType<IPublicPricingProvider>();
            if (p.Success)
                GetPricing(p.Provider, pairs, firstPriceLessThan1);
        }

        public virtual void TestGetVolume() { }
        public void TestGetVolume(List<AssetPair> pairs)
        {
            var p = IsType<IPublicVolumeProvider>();
            if (p.Success)
                GetVolume(p.Provider, pairs);
        }

        public virtual void TestGetAssetPairs() { }
        public void TestGetAssetPairs(AssetPairs requiredPairs)
        {
            var p = IsType<IAssetPairsProvider>();
            if (p.Success)
                GetAssetPairs(p.Provider, requiredPairs);
        }

        public virtual void TestGetBalances()
        {
            var p = IsType<IBalanceProvider>();
            if (p.Success)
                GetBalances(p.Provider);
        }

        public virtual void TestGetAddresses() { }
        public void TestGetAddresses(WalletAddressContext context)
        {
            var p = IsType<IDepositProvider>();
            if (p.Success)
                GetAddresses(p.Provider, context);
        }


        public virtual void TestGetAddressesForAsset() { }
        public void TestGetAddressesForAsset(WalletAddressAssetContext context)
        {
            var p = IsType<IDepositProvider>();
            if (p.Success)
                GetAddressesForAsset(p.Provider, context);
        }

        public virtual void TestGetOrderBook() { }
        public void TestGetOrderBook(OrderBookContext context)
        {
            var p = IsType<IOrderBookProvider>();
            if (p.Success)
                GetOrderBook(p.Provider, context);
        }

        public virtual void TestGetWithdrawalHistory() { }
        public void TestGetWithdrawalHistory(WithdrawalHistoryContext context)
        {
            var p = IsType<IWithdrawalHistoryProvider>();
            if (p.Success)
                GetWithdrawalHistory(p.Provider, context);
        }


        public virtual void TestPlaceWithdrawal() { }
        public void TestPlaceWithdrawal(WithdrawalPlacementContext context)
        {
            var p = IsType<IWithdrawalPlacementProvider>();
            if (p.Success)
                PlaceWithdrawal(p.Provider, context);
        }

        public virtual void TestPlaceWithdrawalExtended() { }
        public void TestPlaceWithdrawalExtended(WithdrawalPlacementContextExtended context)
        {
            var p = IsType<IWithdrawalPlacementProviderExtended>();
            if (p.Success)
                PlaceWithdrawalExtended(p.Provider, context);
        }

        public virtual void TestCancelWithdrawal() { }
        public void TestCancelWithdrawal(WithdrawalCancelationContext context)
        {
            var p = IsType<IWithdrawalCancelationProvider>();
            if (p.Success)
                CancelWithdrawal(p.Provider, context);
        }


        public virtual void TestConfirmWithdrawal() { }
        public void TestConfirmWithdrawal(WithdrawalConfirmationContext context)
        {
            var p = IsType<IWithdrawalConfirmationProvider>();
            if (p.Success)
                ConfirmWithdrawal(p.Provider, context);
        }

        #endregion

        #region Test methods


        private void TestApi(INetworkProviderPrivate provider)
        {
            var ctx = new ApiPrivateTestContext(UserContext.Current.GetApiKey(provider));

            try
            {
                var r = AsyncContext.Run(() => provider.TestPrivateApiAsync(ctx));
                Assert.IsTrue(r);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void TestPublicApi(INetworkProvider provider)
        {
            var ctx = new NetworkProviderContext();

            try
            {
                var r = AsyncContext.Run(() => provider.TestPublicApiAsync(ctx));
                Assert.IsTrue(r);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void GetOhlcAsync(IOhlcProvider provider, OhlcContext context)
        {
            if (context == null)
                return;

            try
            {
                var ohlc = AsyncContext.Run(() => provider.GetOhlcAsync(context));

                bool success = true;
                OhlcEntry ohlcEntryPrev = null;

                foreach (var ohlcEntry in ohlc)
                {
                    if (ohlcEntryPrev != null)
                    {
                        if (ohlcEntry.DateTimeUtc >= ohlcEntryPrev.DateTimeUtc)
                        {
                            success = false;
                            Assert.Fail("Time check is failed.");
                        }
                    }
                    ohlcEntryPrev = ohlcEntry;
                }

                Assert.IsTrue(success);
                Assert.IsTrue(ohlc != null && ohlc.Count > 0);

                Trace.WriteLine("OHLC data:");
                foreach (var entry in ohlc)
                {
                    Trace.WriteLine($"{entry.DateTimeUtc}: O {entry.Open}, H {entry.High}, L {entry.Low}, C {entry.Close}");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void InternalGetPriceAsync(IPublicPricingProvider provider, PublicPricesContext context, bool firstPriceLessThan1, bool runSingle)
        {
            Assert.IsTrue(provider.PricingFeatures != null, "Pricing features object is null");

            var r = AsyncContext.Run(() => provider.GetPricingAsync(context));

            Assert.IsTrue(runSingle ? r.WasViaSingleMethod : !r.WasViaSingleMethod,
                runSingle
                    ? "Single price request was completed using multiple prices endpoint"
                    : "Multiple price request was completed using single price endpoint");
            Assert.IsTrue(r.IsCompleted, "Request is not completed. Missing pairs: " + string.Join(",", r.MissedPairs));

            Assert.IsTrue(r.FirstPrice != null, "First price is null");

            if(context.IsRequestAll)
                Assert.IsTrue(!context.Pairs.Any(), "Context should not have any pairs when requesting prices for all supported by exchange pairs");
            else
            { 
                Assert.IsTrue(r.FirstPrice.QuoteAsset.Equals(context.Pair.Asset1), "Incorrect base asset");
                Assert.IsTrue(r.FirstPrice.Price.Asset.Equals(context.Pair.Asset2), "Incorrect quote asset");

                Assert.IsTrue(r.MarketPrices.DistinctBy(x => x.Pair).Count() == context.Pairs.Count, "Pair duplicates found");
            }

            if (context.IsRequestAll == false)
            {
                if (firstPriceLessThan1) // Checks if the pair is reversed (price-wise).
                    Assert.IsTrue(r.FirstPrice.Price < 1, "Reverse check failed. Price is expected to be < 1");
                else
                    Assert.IsTrue(r.FirstPrice.Price > 1, "Reverse check failed. Price is expected to be > 1");
            }

            Trace.WriteLine($"First asset: {r.FirstPrice}");

            var pricingFeatures = runSingle ? provider.PricingFeatures.Single : provider.PricingFeatures.Bulk as PricingFeaturesItemBase;

            foreach (var p in r.MarketPrices)
            {
                Trace.WriteLine($"Market price: {p}");

                if (pricingFeatures.CanStatistics)
                {
                    Assert.IsTrue(p.HasStatistics,
                        $"Market price does not have statistics but provider supports it - {p.Pair}");

                    Trace.WriteLine($"Market price statistics for {p.Pair}:");

                    Trace.WriteLine(
                        $"Bid: {(p.PriceStatistics.HasHighestBid ? p.PriceStatistics.HighestBid.Display : "-")}");
                    Trace.WriteLine(
                        $"Ask: {(p.PriceStatistics.HasLowestAsk ? p.PriceStatistics.LowestAsk.Display : "-")}");
                    Trace.WriteLine(
                        $"Low: {(p.PriceStatistics.HasPrice24Low ? p.PriceStatistics.Price24Low.Display : "-")}");
                    Trace.WriteLine(
                        $"High: {(p.PriceStatistics.HasPrice24High ? p.PriceStatistics.Price24High.Display : "-")}");
                }
                else
                {
                    Assert.IsTrue(!p.HasStatistics, $"Provider returns statistics but did not announce it - {p.Pair}");
                }

                if (pricingFeatures.CanVolume)
                {
                    Assert.IsTrue(p.HasVolume,
                        $"Market price does not have volume but provider supports it - {p.Pair}");

                    if (p.Volume.HasVolume24Base)
                        Trace.WriteLine($"Base 24h volume: {p.Volume.Volume24Base}");

                    if (p.Volume.HasVolume24Quote)
                        Trace.WriteLine($"Quote 24h volume: {p.Volume.Volume24Quote}");
                }
                else
                {
                    Assert.IsTrue(!p.HasVolume, $"Provider returns volume but did not announce it - {p.Pair}");
                }

                Trace.WriteLine("");
            }
        }

        private void GetPricing(IPublicPricingProvider provider, List<AssetPair> pairs, bool firstPriceLessThan1)
        {
            try
            {
                Trace.WriteLine("Pricing interface test\n\n");

                if (provider.PricingFeatures.HasSingle)
                {
                    Trace.WriteLine("\nSingle features test\n");
                    var context = new PublicPriceContext(pairs.First())
                    {
                        RequestStatistics = provider.PricingFeatures.Single.CanStatistics,
                        RequestVolume = provider.PricingFeatures.Single.CanVolume
                    };

                    InternalGetPriceAsync(provider, context, firstPriceLessThan1, true);
                }

                if (provider.PricingFeatures.HasBulk)
                {
                    Trace.WriteLine("\nBulk features test with pairs selection\n");
                    var context = new PublicPricesContext(pairs)
                    {
                        RequestStatistics = provider.PricingFeatures.Bulk.CanStatistics,
                        RequestVolume = provider.PricingFeatures.Bulk.CanVolume
                    };

                    InternalGetPriceAsync(provider, context, firstPriceLessThan1, false);

                    if (provider.PricingFeatures.Bulk.CanReturnAll)
                    {
                        Trace.WriteLine("\nBulk features test (provider can return all prices)\n");
                        context = new PublicPricesContext();

                        InternalGetPriceAsync(provider, context, firstPriceLessThan1, false);
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }


        private void InternalGetVolumeAsync(IPublicVolumeProvider provider, PublicVolumesContext context, bool runSingle)
        {
            Assert.IsTrue(provider.VolumeFeatures != null, "Volume features object is null");

            var r = AsyncContext.Run(() => provider.GetPublicVolumeAsync(context));

            Assert.IsTrue(runSingle ? r.WasViaSingleMethod : !r.WasViaSingleMethod,
                runSingle
                    ? "Single volume request was completed using multiple prices endpoint"
                    : "Multiple volume request was completed using single price endpoint");
        }

        private void GetVolume(IPublicVolumeProvider provider, List<AssetPair> pairs)
        {
            try
            {
                Trace.WriteLine("Volume interface test\n\n");

                if (provider.VolumeFeatures.HasSingle)
                {
                    Trace.WriteLine("\nSingle features test\n");

                    var context = new PublicVolumeContext(pairs.First());

                    InternalGetVolumeAsync(provider, context, true);
                }

                if (provider.VolumeFeatures.HasBulk)
                {
                    Trace.WriteLine("\nBulk features test with pairs selection\n");
                    var context = new PublicVolumesContext(pairs);

                    InternalGetVolumeAsync(provider, context, false);

                    if (provider.VolumeFeatures.Bulk.CanReturnAll)
                    {
                        Trace.WriteLine("\nBulk features test (provider can return all volumes)\n");
                        context = new PublicVolumesContext();

                        InternalGetVolumeAsync(provider, context, false);
                    }
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected void GetAssetPairs(IAssetPairsProvider provider, AssetPairs requiredPairs)
        {
            var ctx = new NetworkProviderContext();

            try
            {
                var pairs = AsyncContext.Run(() => provider.GetAssetPairsAsync(ctx));

                Assert.IsTrue(pairs != null);
                Assert.IsTrue(pairs.Count > 0);

                if (requiredPairs != null)
                {
                    Trace.WriteLine("Checked pairs:");
                    foreach (var requiredPair in requiredPairs)
                    {
                        var contains = pairs.Contains(requiredPair);
                        Assert.IsTrue(contains, $"Provider didn't return required {requiredPair} pair.");

                        if (contains)
                            Trace.WriteLine(requiredPair);
                    }
                }

                Trace.WriteLine("\nRemote pairs from exchange:");
                foreach (var pair in pairs)
                {
                    Trace.WriteLine(pair);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void GetBalances(IBalanceProvider provider)
        {
            var ctx = new NetworkProviderPrivateContext(UserContext.Current);

            try
            {
                var balances = AsyncContext.Run(() => provider.GetBalancesAsync(ctx));

                Assert.IsTrue(balances != null);

                Trace.WriteLine("User balances: ");
                foreach (var b in balances)
                {
                    Trace.WriteLine($"{b.Asset}: {b.Available} available, {b.Balance} balance, {b.Reserved} reserved");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void GetAddresses(IDepositProvider provider, WalletAddressContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = AsyncContext.Run(() => provider.GetAddressesAsync(context));

                Assert.IsTrue(r != null);

                Trace.WriteLine("All deposit addresses:");
                foreach (var walletAddress in r)
                {
                    Trace.WriteLine($"{walletAddress.Asset}: \"{walletAddress.Address}\"");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void GetAddressesForAsset(IDepositProvider provider, WalletAddressAssetContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = AsyncContext.Run(() => provider.GetAddressesForAssetAsync(context));

                Assert.IsTrue(r != null);

                Trace.WriteLine($"Deposit addresses for {context.Asset}:");
                foreach (var walletAddress in r)
                {
                    Trace.WriteLine($"\"{walletAddress.Address}\"");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void GetOrderBook(IOrderBookProvider provider, OrderBookContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = AsyncContext.Run(() => provider.GetOrderBookAsync(context));
                Assert.IsTrue(r != null);

                if (context.MaxRecordsCount.HasValue)
                    Assert.IsTrue(r.Count == context.MaxRecordsCount.Value);
                else
                    Assert.IsTrue(r.Count > 0);

                Trace.WriteLine($"Order book data ({r.Count(x => x.Type == OrderType.Ask)} asks, {r.Count(x => x.Type == OrderType.Bid)} bids): ");
                foreach (var obr in r)
                {
                    Trace.WriteLine($"{obr.UtcUpdated} | For {context.Pair.Asset1}: {obr.Type} {obr.Price.Display}, {obr.Volume} ");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void GetWithdrawalHistory(IWithdrawalHistoryProvider provider, WithdrawalHistoryContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = AsyncContext.Run(() => provider.GetWithdrawalHistoryAsync(context));

                foreach (var historyEntry in r)
                {
                    Trace.WriteLine($"{historyEntry.CreatedTimeUtc} {historyEntry.WithdrawalStatus} {historyEntry.WithdrawalRemoteId} {historyEntry.Price.Display}");
                }

                // Assert.IsTrue(r);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void PlaceWithdrawal(IWithdrawalPlacementProvider provider, WithdrawalPlacementContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = AsyncContext.Run(() => provider.PlaceWithdrawalAsync(context));

                // Assert.IsTrue(r);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void PlaceWithdrawalExtended(IWithdrawalPlacementProviderExtended provider, WithdrawalPlacementContextExtended context)
        {
            if (context == null)
                return;

            try
            {
                var r = AsyncContext.Run(() => provider.PlaceWithdrawalAsync(context));

                Assert.IsTrue(r != null);

                Trace.WriteLine($"Withdrawal request remote id: {r.WithdrawalRemoteId}");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void CancelWithdrawal(IWithdrawalCancelationProvider provider, WithdrawalCancelationContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = AsyncContext.Run(() => provider.CancelWithdrawalAsync(context));

                Assert.IsTrue(r != null);
                Assert.IsTrue(r.WithdrawalRemoteId.Equals(context.WithdrawalRemoteId), "Withdrawal ids don't match.");

                Trace.WriteLine($"Withdrawal request canceled, remote id is {r.WithdrawalRemoteId}");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void ConfirmWithdrawal(IWithdrawalConfirmationProvider provider, WithdrawalConfirmationContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = AsyncContext.Run(() => provider.ConfirmWithdrawalAsync(context));

                Assert.IsTrue(r != null);
                Assert.IsTrue(r.WithdrawalRemoteId.Equals(context.WithdrawalRemoteId), "Withdrawal ids don't match.");

                Trace.WriteLine($"Withdrawal request confirmed, remote id is {r.WithdrawalRemoteId}");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        #endregion

        #region Utilities

        protected (bool Success, T Provider) IsType<T>()
        {
            if (!(Provider is T tp))
                Assert.Fail(Provider.Title + " is not of type " + typeof(T).Name);
            else
                return (true, tp);
            return (false, default(T));
        }

        #endregion

        /*
        public void TestPortfolioAccountBalances()
        {
            var provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();

            var c = new PortfolioProviderContext(UserContext.Current, provider, UserContext.Current.BaseAsset, 0);
            var scanner = new PortfolioProvider(c);
            try
            {
                scanner.Update();
                foreach (var i in scanner.Items)
                    Console.WriteLine(i.Asset.ShortCode + " " + i.AvailableBalance);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } 
        */
    }
}