using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Common.Wallet.Withdrawal.Cancelation;
using Prime.Common.Wallet.Withdrawal.History;

namespace Prime.Tests.Providers
{
    public abstract class ProviderDirectTestsBase
    {
        public INetworkProvider Provider { get; protected set; }

        #region Wrappers

        public virtual async Task TestApiAsync()
        {
            var p = IsType<INetworkProviderPrivate>();
            if (p.Success)
                await TestApiAsync(p.Provider);
        }

        public virtual async Task TestGetOhlcAsync()
        {
            var p = IsType<IOhlcProvider>();
            if (p.Success)
                await GetOhlcAsync(p.Provider);
        }

        public virtual async Task TestGetPriceAsync()
        {
            var p = IsType<IPublicPriceProvider>();
            if (p.Success)
                await GetPairPriceAsync(p.Provider);
        }

        public virtual async Task TestGetAssetPricesAsync()
        {
            var p = IsType<IPublicAssetPricesProvider>();
            if (p.Success)
                await GetAssetPricesAsync(p.Provider);
        }

        public virtual async Task TestGetPricesAsync()
        {
            var p = IsType<IPublicPricesProvider>();
            if (p.Success)
                await GetPricesAsync(p.Provider);
        }

        public virtual async Task TestGetAssetPairsAsync()
        {
            var p = IsType<IExchangeProvider>();
            if (p.Success)
                await GetAssetPairsAsync(p.Provider);
        }

        public virtual async Task TestGetBalancesAsync()
        {
            var p = IsType<IWalletService>();
            if (p.Success)
                await GetBalancesAsync(p.Provider);
        }

        public virtual async Task TestGetAddressesAsync()
        {
            var p = IsType<IDepositService>();
            if (p.Success)
                await GetAddressesAsync(p.Provider);
        }

        public virtual async Task TestGetAddressesForAssetAsync()
        {
            var p = IsType<IDepositService>();
            if (p.Success)
                await GetAddressesForAssetAsync(p.Provider);
        }

        public virtual async Task TestGetOrderBookAsync()
        {
            var p = IsType<IOrderBookProvider>();
            if (p.Success)
                await GetOrderBookAsync(p.Provider);
        }

        public virtual async Task TestGetWithdrawalHistoryAsync()
        {
            var p = IsType<IWithdrawalHistoryProvider>();
            if (p.Success)
                await GetWithdrawalHistoryAsync(p.Provider);
        }

        public virtual async Task TestPlaceWithdrawalAsync()
        {
            var p = IsType<IWithdrawalPlacementProvider>();
            if (p.Success)
                await PlaceWithdrawalAsync(p.Provider);
        }

        public virtual async Task TestPlaceWithdrawalExtendedAsync()
        {
            var p = IsType<IWithdrawalPlacementProviderExtended>();
            if (p.Success)
                await PlaceWithdrawalExtendedAsync(p.Provider);
        }

        public virtual async Task TestCancelWithdrawalAsync()
        {
            var p = IsType<IWithdrawalCancelationProvider>();
            if (p.Success)
                await CancelWithdrawalAsync(p.Provider);
        }

        #endregion

        #region Test methods


        private async Task TestApiAsync(INetworkProviderPrivate provider)
        {
            var ctx = new ApiTestContext(UserContext.Current.GetApiKey(provider));

            try
            {
                var r = await provider.TestApiAsync(ctx);
                Assert.IsTrue(r);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected OhlcContext OhlcContext { get; set; }

        private async Task GetOhlcAsync(IOhlcProvider provider)
        {
            if (OhlcContext == null)
            {
                OhlcContext = new OhlcContext(new AssetPair("BTC", "USD"), TimeResolution.Minute,
                    new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Minute), null);
            }

            try
            {
                var ohlc = await provider.GetOhlcAsync(OhlcContext);

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
                            break;
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

        protected PublicPriceContext PublicPriceContext { get; set; }

        private async Task GetPairPriceAsync(IPublicPriceProvider provider)
        {
            if (PublicPriceContext == null)
            {
                PublicPriceContext = new PublicPriceContext(new AssetPair("BTC", "USD"));
            }

            try
            {
                var c = await provider.GetPriceAsync(PublicPriceContext);

                Assert.IsTrue(c != null);
                Assert.IsTrue(c.QuoteAsset.Equals(PublicPriceContext.Pair.Asset1));
                Assert.IsTrue(c.Price.Asset.Equals(PublicPriceContext.Pair.Asset2));

                Trace.WriteLine($"Quote asset: {c.QuoteAsset}, Price: {c.Price.Display}");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected PublicAssetPricesContext PublicAssetPricesContext { get; set; }

        private async Task GetAssetPricesAsync(IPublicAssetPricesProvider provider)
        {
            if (PublicAssetPricesContext == null)
            {
                PublicAssetPricesContext = new PublicAssetPricesContext(new List<Asset>()
                {
                    "USD".ToAssetRaw(),
                    "EUR".ToAssetRaw()
                }, Asset.Btc);
            }

            try
            {
                var c = await provider.GetAssetPricesAsync(PublicAssetPricesContext);

                Assert.IsNotNull(c);
                Assert.IsTrue(c.MarketPrices.Count == PublicAssetPricesContext.Assets.Count);

                foreach (var requiredAsset in PublicAssetPricesContext.Assets)
                {
                    Assert.IsTrue(
                        c.MarketPrices.Exists(
                            x => x.QuoteAsset.Equals(requiredAsset) &&
                                 x.Price.Asset.Equals(PublicAssetPricesContext.QuoteAsset)
                        ),
                        $"Provider did not return {requiredAsset.ToPair(PublicAssetPricesContext.QuoteAsset)} price"
                    );
                }

                foreach (var price in c.MarketPrices)
                {
                    Trace.WriteLine($"Latest price for {price.QuoteAsset}: {price.Price.Display}");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected AssetPairs RequiredAssetPairs { get; set; }

        private async Task GetAssetPairsAsync(IExchangeProvider provider)
        {
            var ctx = new NetworkProviderContext();

            try
            {
                var pairs = await provider.GetAssetPairs(ctx);

                Assert.IsTrue(pairs != null);
                Assert.IsTrue(pairs.Count > 0);

                if (RequiredAssetPairs != null)
                {
                    Trace.WriteLine("Checked pairs:");
                    foreach (var requiredPair in RequiredAssetPairs)
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

        protected PublicPricesContext PublicPricesContext { get; set; }

        private async Task GetPricesAsync(IPublicPricesProvider provider)
        {
            if (PublicPricesContext == null)
            {
                PublicPricesContext = new PublicPricesContext(new List<AssetPair>()
                {
                    "BTC_USD".ToAssetPairRaw(),
                    "BTC_EUR".ToAssetPairRaw()
                });
            }

            try
            {
                var pairs = await provider.GetPricesAsync(PublicPricesContext);

                Assert.IsTrue(pairs != null);
                Assert.IsTrue(pairs.MarketPrices.Count > 0);

                foreach (var requiredPair in PublicPricesContext.Pairs)
                {
                    Assert.IsTrue(pairs.MarketPrices.Exists(x => x.QuoteAsset.Equals(requiredPair.Asset1)), $"Provider did not return {requiredPair} price");
                }

                Trace.WriteLine("Latest prices:");
                foreach (var pair in pairs.MarketPrices)
                {
                    Trace.WriteLine($"Quote asset: {pair.QuoteAsset}, Price: {pair.Price.Display}");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private async Task GetBalancesAsync(IWalletService provider)
        {
            var ctx = new NetworkProviderPrivateContext(UserContext.Current);

            try
            {
                var balances = await provider.GetBalancesAsync(ctx);

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

        protected WalletAddressContext WalletAddressContext { get; set; }

        private async Task GetAddressesAsync(IDepositService provider)
        {
            if (WalletAddressContext == null)
            {
                WalletAddressContext = new WalletAddressContext(UserContext.Current);
            }

            try
            {
                var r = await provider.GetAddressesAsync(WalletAddressContext);

                Assert.IsTrue(r != null);

                Trace.WriteLine($"All deposit addresses:");
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

        protected WalletAddressAssetContext WalletAddressAssetContext { get; set; }

        private async Task GetAddressesForAssetAsync(IDepositService provider)
        {
            if (WalletAddressAssetContext == null)
            {
                WalletAddressAssetContext = new WalletAddressAssetContext("BTC".ToAsset(provider), UserContext.Current);
            }

            try
            {
                var r = await provider.GetAddressesForAssetAsync(WalletAddressAssetContext);

                Assert.IsTrue(r != null);

                Trace.WriteLine($"Deposit addresses for {WalletAddressAssetContext.Asset}:");
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

        protected OrderBookContext OrderBookContext { get; set; }

        private async Task GetOrderBookAsync(IOrderBookProvider provider)
        {
            if (OrderBookContext == null)
                OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()));

            try
            {
                var r = await provider.GetOrderBook(OrderBookContext);
                Assert.IsTrue(r != null);

                if (OrderBookContext.MaxRecordsCount.HasValue)
                    Assert.IsTrue(r.Count == OrderBookContext.MaxRecordsCount.Value);
                else
                    Assert.IsTrue(r.Count > 0);

                Trace.WriteLine($"Order book data ({r.Count(x => x.Type == OrderBookType.Ask)} asks, {r.Count(x => x.Type == OrderBookType.Bid)} bids): ");
                foreach (var obr in r)
                {
                    Trace.WriteLine($"{obr.Data.Time} | For {OrderBookContext.Pair.Asset1}: {obr.Type} {obr.Data.Price.Display}, {obr.Data.Volume} ");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected WithdrawalHistoryContext WithdrawalHistoryContext { get; set; }

        private async Task GetWithdrawalHistoryAsync(IWithdrawalHistoryProvider provider)
        {
            if (WithdrawalHistoryContext == null)
            {
                WithdrawalHistoryContext = new WithdrawalHistoryContext(UserContext.Current)
                {
                    Asset = Asset.Btc,
                    FromTimeUtc = DateTime.UtcNow.AddDays(-7)
                };
            }

            try
            {
                var r = await provider.GetWithdrawalHistory(WithdrawalHistoryContext);

                // Assert.IsTrue(r);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected WithdrawalPlacementContext WithdrawalPlacementContext { get; set; }

        private async Task PlaceWithdrawalAsync(IWithdrawalPlacementProvider provider)
        {
            if (WithdrawalPlacementContext == null)
                throw new NullReferenceException($"{nameof(WithdrawalPlacementContext)} should not be bull");

            try
            {
                var r = await provider.PlaceWithdrawal(WithdrawalPlacementContext);

                // Assert.IsTrue(r);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected WithdrawalPlacementContextExtended WithdrawalPlacementContextExtended { get; set; }

        private async Task PlaceWithdrawalExtendedAsync(IWithdrawalPlacementProviderExtended provider)
        {
            if (WithdrawalPlacementContextExtended == null)
                throw new NullReferenceException($"{nameof(WithdrawalPlacementContextExtended)} should not be bull");

            try
            {
                var r = await provider.PlaceWithdrawal(WithdrawalPlacementContextExtended);

                Assert.IsTrue(r != null);

                Trace.WriteLine($"Withdrawal request remote id: {r.WithdrawalRemoteId}");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected WithdrawalCancelationContext WithdrawalCancelationContext { get; set; }

        private async Task CancelWithdrawalAsync(IWithdrawalCancelationProvider provider)
        {
            if (WithdrawalCancelationContext == null)
                throw new NullReferenceException($"{nameof(WithdrawalCancelationContext)} should not be bull");

            try
            {
                var r = await provider.CancelWithdrawal(WithdrawalCancelationContext);

                Assert.IsTrue(r != null);
                Assert.IsTrue(r.WithdrawalRemoteId.Equals(WithdrawalCancelationContext.WithdrawalRemoteId), "Withdrawal remote ids don't match.");

                Trace.WriteLine($"Canceled request remote id: {r.WithdrawalRemoteId}");
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
                Assert.Fail(Provider.Title + " is not of type " + nameof(INetworkProviderPrivate));
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