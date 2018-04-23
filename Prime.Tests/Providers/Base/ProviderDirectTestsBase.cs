using System;
using System.Collections.Generic;
using System.Linq;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Common.Wallet.Withdrawal.Cancelation;
using Prime.Common.Wallet.Withdrawal.Confirmation;
using Prime.Common.Wallet.Withdrawal.History;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public abstract partial class ProviderDirectTestsBase
    {
        protected ITestOutputHelper OutputWriter;

        protected ProviderDirectTestsBase(ITestOutputHelper outputWriter)
        {
            OutputWriter = outputWriter;
        }

        public INetworkProvider Provider { get; protected set; }

        private bool IsVolumePricingSanityTested { get; set; }

        #region Wrappers

        public virtual void TestApiPrivate()
        {
            var p = IsType<INetworkProviderPrivate>();
            if (p.Success)
                ApiPrivateTest(p.Provider);
        }

        public virtual void TestApiPublic()
        {
            var p = IsType<INetworkProvider>();
            if (p.Success)
                ApiPublicTest(p.Provider);
        }


        public virtual void TestGetOhlc() { }
        public void PretestGetOhlc(OhlcContext context)
        {
            var p = IsType<IOhlcProvider>();
            if (p.Success)
                GetOhlcTest(p.Provider, context);
        }

        public virtual void TestGetAssetPairs() { }
        public void PretestGetAssetPairs(AssetPairs requiredPairs)
        {
            var p = IsType<IAssetPairsProvider>();
            if (p.Success)
                GetAssetPairsTest(p.Provider, requiredPairs);
        }

        public virtual void TestGetAddresses() { }
        public void PretestGetAddresses(WalletAddressContext context)
        {
            var p = IsType<IDepositProvider>();
            if (p.Success)
                GetAddressesTest(p.Provider, context);
        }


        public virtual void TestGetAddressesForAsset() { }
        public void PretestGetAddressesForAsset(WalletAddressAssetContext context)
        {
            var p = IsType<IDepositProvider>();
            if (p.Success)
                GetAddressesForAssetTest(p.Provider, context);
        }

        public virtual void TestGetOrderBook() { }
        public void PretestGetOrderBook(AssetPair pair, bool priceLessThan1, int recordsCount = 100)
        {
            var p = IsType<IOrderBookProvider>();
            if (p.Success)
                GetOrderBookTest(p.Provider, pair, priceLessThan1, recordsCount);
        }

        public virtual void TestGetWithdrawalHistory() { }
        public void PretestGetWithdrawalHistory(WithdrawalHistoryContext context)
        {
            var p = IsType<IWithdrawalHistoryProvider>();
            if (p.Success)
                GetWithdrawalHistoryTest(p.Provider, context);
        }


        public virtual void TestPlaceWithdrawal() { }
        public void PretestPlaceWithdrawal(WalletAddress address, Money amount, string description = null, Money? customFee = null, string authToken = null)
        {
            var p = IsType<IWithdrawalPlacementProvider>();
            if (p.Success)
                PlaceWithdrawalTest(p.Provider, address, amount, description, customFee, authToken);
        }

        public virtual void TestCancelWithdrawal() { }
        public void PretestCancelWithdrawal(WithdrawalCancelationContext context)
        {
            var p = IsType<IWithdrawalCancelationProvider>();
            if (p.Success)
                CancelWithdrawalTest(p.Provider, context);
        }


        public virtual void TestConfirmWithdrawal() { }
        public void PretestConfirmWithdrawal(WithdrawalConfirmationContext context)
        {
            var p = IsType<IWithdrawalConfirmationProvider>();
            if (p.Success)
                ConfirmWithdrawalTest(p.Provider, context);
        }

        #endregion

        #region Test methods


        private void TestVolumePricingSanity(INetworkProvider provider, List<AssetPair> pairs)
        {
            // TODO: AY: to be reviewed and tested when there are new providers that support Volume and Pricing interfaces.

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
            Assert.False(
                price.FirstPrice.Volume.HasVolume24Base &&
                canVolumeBase,
                "Provider returns base volume using both pricing and volume interfaces");
            Assert.False(
                price.FirstPrice.Volume.HasVolume24Quote &&
                canVolumeQuote,
                "Provider returns quote volume using both pricing and volume interfaces");
        }

        private void ApiPrivateTest(INetworkProviderPrivate provider)
        {
            var ctx = new ApiPrivateTestContext(UserContext.Current.GetApiKey(provider));

            var r = AsyncContext.Run(() => provider.TestPrivateApiAsync(ctx));
            Assert.True(r);
        }
        
        private void ApiPublicTest(INetworkProvider provider)
        {
            var ctx = new NetworkProviderContext();

            var r = AsyncContext.Run(() => provider.TestPublicApiAsync(ctx));
            Assert.True(r);
        }

        private void GetOhlcTest(IOhlcProvider provider, OhlcContext context)
        {
            if (context == null)
                return;

            var ohlc = AsyncContext.Run(() => provider.GetOhlcAsync(context));

            bool success = true;
            OhlcEntry ohlcEntryPrev = null;

            foreach (var ohlcEntry in ohlc.OhlcData)
            {
                if (ohlcEntryPrev != null)
                {
                    if (ohlcEntry.DateTimeUtc >= ohlcEntryPrev.DateTimeUtc)
                    {
                        success = false;
                        throw new InvalidOperationException("Time check is failed.");
                    }
                }
                ohlcEntryPrev = ohlcEntry;
            }

            Assert.True(success);
            Assert.True(ohlc != null && ohlc.OhlcData.Count > 0);

            OutputWriter.WriteLine("OHLC data:");
            foreach (var entry in ohlc.OhlcData)
            {
                OutputWriter.WriteLine($"{entry.DateTimeUtc}: O {entry.Open}, H {entry.High}, L {entry.Low}, C {entry.Close}");
            }
        }

        protected void GetAssetPairsTest(IAssetPairsProvider provider, AssetPairs requiredPairs)
        {
            var ctx = new NetworkProviderContext();

            var pairs = AsyncContext.Run(() => provider.GetAssetPairsAsync(ctx));

            Assert.True(pairs != null);
            Assert.True(pairs.Count > 0);

            if (requiredPairs != null)
            {
                OutputWriter.WriteLine("Checked pairs:");
                foreach (var requiredPair in requiredPairs)
                {
                    var contains = pairs.Contains(requiredPair);
                    Assert.True(contains, $"Provider didn't return required {requiredPair} pair.");

                    OutputWriter.WriteLine(requiredPair.ToString());
                }
            }

            OutputWriter.WriteLine("\nRemote pairs from exchange:");
            foreach (var pair in pairs)
            {
                OutputWriter.WriteLine(pair.ToString());
            }
        }

        private void GetAddressesTest(IDepositProvider provider, WalletAddressContext context)
        {
            if (context == null)
                return;

            var r = AsyncContext.Run(() => provider.GetAddressesAsync(context));

            Assert.True(r != null);

            OutputWriter.WriteLine("All deposit addresses:");
            foreach (var walletAddress in r.WalletAddresses)
            {
                OutputWriter.WriteLine($"{walletAddress.Asset}: \"{walletAddress.Address}\"");
            }
        }

        private void GetAddressesForAssetTest(IDepositProvider provider, WalletAddressAssetContext context)
        {
            if (context == null)
                return;

            var r = AsyncContext.Run(() => provider.GetAddressesForAssetAsync(context));

            Assert.True(r != null);

            OutputWriter.WriteLine($"Deposit addresses for {context.Asset}:");
            foreach (var walletAddress in r.WalletAddresses)
            {
                OutputWriter.WriteLine($"\"{walletAddress.Address}\"");
            }
        }

        private void InternalGetOrderBook(IOrderBookProvider provider, OrderBookContext context, bool priceLessThan1)
        {
            var r = AsyncContext.Run(() => provider.GetOrderBookAsync(context));
            Assert.True(r != null, "Null response returned");

            if (r.IsReversed)
                OutputWriter.WriteLine("Asset pair is reversed");

            // Assert.True(r.Pair.Equals(context.Pair), "Incorrect asset pair returned");

            Assert.True(r.Asks.Count > 0, "No asks returned");
            Assert.True(r.Bids.Count > 0, "No bids returned");
            Assert.True(r.Asks.Count <= context.MaxRecordsCount, "Incorrect number of ask order book records returned");
            Assert.True(r.Bids.Count <= context.MaxRecordsCount, "Incorrect number of bid order book records returned");

            //if (context.MaxRecordsCount == Int32.MaxValue)
            //    Assert.True(r.Count > 0, "No order book records returned");
            //else
            //    Assert.True(r.Asks.Count == context.MaxRecordsCount && r.Bids.Count == context.MaxRecordsCount, "Incorrect number of order book records returned");

            OutputWriter.WriteLine($"Highest bid: {r.HighestBid}");
            OutputWriter.WriteLine($"Lowest ask: {r.LowestAsk}");

            var records = new List<OrderBookRecord>() { r.LowestAsk, r.HighestBid };
            foreach (var record in records)
            {
                if (priceLessThan1) // Checks if the pair is reversed (price-wise).
                    Assert.True(record.Price < 1, "Reverse check failed. Price is expected to be < 1");
                else
                    Assert.True(record.Price > 1, "Reverse check failed. Price is expected to be > 1");
            }

            OutputWriter.WriteLine($"Order book data ({r.Asks.Count} asks, {r.Bids.Count} bids): ");

            foreach (var obr in r.Asks.Concat(r.Bids))
            {
                OutputWriter.WriteLine($"{obr.UtcUpdated} : {obr}");
            }
        }

        private void GetOrderBookTest(IOrderBookProvider provider, AssetPair pair, bool priceLessThan1, int recordsCount = 100)
        {
            OutputWriter.WriteLine($"Order book market: {pair}");
            var context = new OrderBookContext(pair, recordsCount);
            InternalGetOrderBook(provider, context, priceLessThan1);

            context = new OrderBookContext(pair, Int32.MaxValue);
            InternalGetOrderBook(provider, context, priceLessThan1);
        }

        private void GetWithdrawalHistoryTest(IWithdrawalHistoryProvider provider, WithdrawalHistoryContext context)
        {
            if (context == null)
                return;

            var r = AsyncContext.Run(() => provider.GetWithdrawalHistoryAsync(context));

            foreach (var historyEntry in r)
            {
                OutputWriter.WriteLine($"{historyEntry.CreatedTimeUtc} {historyEntry.WithdrawalStatus} {historyEntry.WithdrawalRemoteId} {historyEntry.Price.Display}");
            }

            // Assert.True(r);
        }

        private void PlaceWithdrawalTest(IWithdrawalPlacementProvider provider, WalletAddress address, Money amount, string description = null, Money? customFee = null, string authToken = null)
        {
            var context = new WithdrawalPlacementContext(address, amount, UserContext.Current)
            {
                Description = description,
                CustomFee = customFee,
                AuthenticationToken = authToken
            };

            var r = AsyncContext.Run(() => provider.PlaceWithdrawalAsync(context));

            Assert.True(r != null);

            OutputWriter.WriteLine($"Withdrawal request remote id: {r.WithdrawalRemoteId}");
        }

        private void CancelWithdrawalTest(IWithdrawalCancelationProvider provider, WithdrawalCancelationContext context)
        {
            if (context == null)
                return;

            var r = AsyncContext.Run(() => provider.CancelWithdrawalAsync(context));

            Assert.True(r != null);
            Assert.True(r.WithdrawalRemoteId.Equals(context.WithdrawalRemoteId), "Withdrawal ids don't match.");

            OutputWriter.WriteLine($"Withdrawal request canceled, remote id is {r.WithdrawalRemoteId}");
        }

        private void ConfirmWithdrawalTest(IWithdrawalConfirmationProvider provider, WithdrawalConfirmationContext context)
        {
            if (context == null)
                return;

            var r = AsyncContext.Run(() => provider.ConfirmWithdrawalAsync(context));

            Assert.True(r != null);
            Assert.True(r.WithdrawalRemoteId.Equals(context.WithdrawalRemoteId), "Withdrawal ids don't match.");

            OutputWriter.WriteLine($"Withdrawal request confirmed, remote id is {r.WithdrawalRemoteId}");
        }

        #endregion

        #region Utilities

        protected (bool Success, T Provider) IsType<T>()
        {
            if (!(Provider is T tp))
                throw new InvalidOperationException(Provider.Title + " is not of type " + typeof(T).Name);
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