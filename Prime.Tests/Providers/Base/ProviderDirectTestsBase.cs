using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Common.Wallet.Withdrawal.Cancelation;
using Prime.Common.Wallet.Withdrawal.Confirmation;
using Prime.Common.Wallet.Withdrawal.History;

namespace Prime.Tests.Providers
{
    public abstract partial class ProviderDirectTestsBase
    {
        public INetworkProvider Provider { get; protected set; }

        private bool IsVolumePricingSanityTested { get; set; }

        #region Wrappers

        public virtual void TestApiPrivate()
        {
            var p = IsType<INetworkProviderPrivate>();
            if (p.Success)
                TestApiPrivate(p.Provider);
        }

        public virtual void TestApiPublic()
        {
            var p = IsType<INetworkProvider>();
            if (p.Success)
                TestApiPublic(p.Provider);
        }


        public virtual void TestGetOhlc() { }
        public void TestGetOhlc(OhlcContext context)
        {
            var p = IsType<IOhlcProvider>();
            if (p.Success)
                GetOhlcAsync(p.Provider, context);
        }

        public virtual void TestGetAssetPairs() { }
        public void TestGetAssetPairs(AssetPairs requiredPairs)
        {
            var p = IsType<IAssetPairsProvider>();
            if (p.Success)
                GetAssetPairs(p.Provider, requiredPairs);
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
        public void TestGetOrderBook(AssetPair pair, bool priceLessThan1, int recordsCount = 100)
        {
            var p = IsType<IOrderBookProvider>();
            if (p.Success)
                GetOrderBook(p.Provider, pair, priceLessThan1, recordsCount);
        }

        public virtual void TestGetWithdrawalHistory() { }
        public void TestGetWithdrawalHistory(WithdrawalHistoryContext context)
        {
            var p = IsType<IWithdrawalHistoryProvider>();
            if (p.Success)
                GetWithdrawalHistory(p.Provider, context);
        }


        public virtual void TestPlaceWithdrawal() { }
        public void TestPlaceWithdrawal(WalletAddress address, Money amount, string description = null, Money? customFee = null, string authToken = null)
        {
            var p = IsType<IWithdrawalPlacementProvider>();
            if (p.Success)
                PlaceWithdrawal(p.Provider, address, amount, description, customFee, authToken);
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


        private void TestVolumePricingSanity(INetworkProvider provider, List<AssetPair> pairs)
        {
            // TODO: to be reviewed and tested when there will be new providers that support Volume and Pricing interfaces.

            if (IsVolumePricingSanityTested)
                return;

            IsVolumePricingSanityTested = true;

            if (provider is IPublicVolumeProvider volumeProvider && provider is IPublicPricingProvider pricingProvider)
            {
                // Single test.

                if (pricingProvider.PricingFeatures.Single != null && volumeProvider.VolumeFeatures.Single != null)
                {
                    if (pricingProvider.PricingFeatures.Single.CanVolume ^
                        volumeProvider.VolumeFeatures.Single.CanVolume)
                        return;

                    var priceCtx = new PublicPriceContext(pairs.First());
                    var rPrice = AsyncContext.Run(() => pricingProvider.GetPricingAsync(priceCtx));

                    AssertPrice(rPrice, volumeProvider.VolumeFeatures.Single.CanVolumeBase,
                        volumeProvider.VolumeFeatures.Single.CanVolumeQuote);
                }

                // Multiple pairs test.

                if (pricingProvider.PricingFeatures.Bulk != null && volumeProvider.VolumeFeatures.Bulk != null)
                {
                    if (pricingProvider.PricingFeatures.Bulk.CanVolume ^ volumeProvider.VolumeFeatures.Bulk.CanVolume)
                        return;

                    var pricesCtx = new PublicPricesContext(pairs);
                    var rPrices = AsyncContext.Run(() => pricingProvider.GetPricingAsync(pricesCtx));

                    AssertPrice(rPrices, volumeProvider.VolumeFeatures.Bulk.CanVolumeBase,
                        volumeProvider.VolumeFeatures.Bulk.CanVolumeQuote);
                }
            }
        }

        private void AssertPrice(MarketPrices price, bool canVolumeBase, bool canVolumeQuote)
        {
            Assert.IsFalse(
                price.FirstPrice.Volume.HasVolume24Base &&
                canVolumeBase,
                "Provider returns base volume using both pricing and volume interfaces");
            Assert.IsFalse(
                price.FirstPrice.Volume.HasVolume24Quote &&
                canVolumeQuote,
                "Provider returns quote volume using both pricing and volume interfaces");
        }

        private void TestApiPrivate(INetworkProviderPrivate provider)
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

        private void TestApiPublic(INetworkProvider provider)
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

        private void InternalGetOrderBook(IOrderBookProvider provider, OrderBookContext context, bool priceLessThan1)
        {
            var r = AsyncContext.Run(() => provider.GetOrderBookAsync(context));
            Assert.IsTrue(r != null, "Null response returned");

            if (r.IsReversed)
                Trace.WriteLine("Asset pair is reversed");

            // Assert.IsTrue(r.Pair.Equals(context.Pair), "Incorrect asset pair returned");

            Assert.IsTrue(r.Asks.Count > 0, "No asks returned");
            Assert.IsTrue(r.Bids.Count > 0, "No bids returned");
            Assert.IsTrue(r.Asks.Count <= context.MaxRecordsCount, "Incorrect number of ask order book records returned");
            Assert.IsTrue(r.Bids.Count <= context.MaxRecordsCount, "Incorrect number of bid order book records returned");

            //if (context.MaxRecordsCount == Int32.MaxValue)
            //    Assert.IsTrue(r.Count > 0, "No order book records returned");
            //else
            //    Assert.IsTrue(r.Asks.Count == context.MaxRecordsCount && r.Bids.Count == context.MaxRecordsCount, "Incorrect number of order book records returned");

            Trace.WriteLine($"Highest bid: {r.HighestBid}");
            Trace.WriteLine($"Lowest ask: {r.LowestAsk}");

            var records = new List<OrderBookRecord>() { r.LowestAsk, r.HighestBid };
            foreach (var record in records)
            {
                if (priceLessThan1) // Checks if the pair is reversed (price-wise).
                    Assert.IsTrue(record.Price < 1, "Reverse check failed. Price is expected to be < 1");
                else
                    Assert.IsTrue(record.Price > 1, "Reverse check failed. Price is expected to be > 1");
            }

            Trace.WriteLine($"Order book data ({r.Asks.Count} asks, {r.Bids.Count} bids): ");

            foreach (var obr in r.Asks.Concat(r.Bids))
            {
                Trace.WriteLine($"{obr.UtcUpdated} : {obr}");
            }
        }

        private void GetOrderBook(IOrderBookProvider provider, AssetPair pair, bool priceLessThan1, int recordsCount = 100)
        {
            Trace.WriteLine($"Order book market: {pair}");
            var context = new OrderBookContext(pair, recordsCount);
            InternalGetOrderBook(provider, context, priceLessThan1);

            context = new OrderBookContext(pair, Int32.MaxValue);
            InternalGetOrderBook(provider, context, priceLessThan1);
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

        private void PlaceWithdrawal(IWithdrawalPlacementProvider provider, WalletAddress address, Money amount, string description = null, Money? customFee = null, string authToken = null)
        {
            var context = new WithdrawalPlacementContext(address, amount, UserContext.Current)
            {
                Description = description,
                CustomFee = customFee,
                AuthenticationToken = authToken
            };

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