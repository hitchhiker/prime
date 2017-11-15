using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Common.Exchange;
using Prime.Common.Wallet.Withdrawal.Cancelation;
using Prime.Common.Wallet.Withdrawal.Confirmation;
using Prime.Common.Wallet.Withdrawal.History;

namespace Prime.Tests.Providers
{
    public abstract class ProviderDirectTestsBase
    {
        [Obsolete]
        protected PublicPriceContext PublicPriceContext { get; set; }

        public INetworkProvider Provider { get; protected set; }

        #region Wrappers

        public virtual async Task TestApiAsync()
        {
            var p = IsType<INetworkProviderPrivate>();
            if (p.Success)
                await TestApiAsync(p.Provider).ConfigureAwait(false);
        }


        public virtual async Task TestGetOhlcAsync() { }
        public async Task TestGetOhlcAsync(OhlcContext context)
        {
            var p = IsType<IOhlcProvider>();
            if (p.Success)
                await GetOhlcAsync(p.Provider, context).ConfigureAwait(false);
        }

        public virtual async Task TestGetPriceAsync() { }
        public async Task TestGetPriceAsync(PublicPriceContext context, bool lessThan1)
        {
            var p = IsType<IPublicPriceSuper>();
            if (p.Success)
                await GetPriceAsync(p.Provider, context, lessThan1).ConfigureAwait(false);
        }

        public virtual async Task TestGetAssetPricesAsync() { }
        public virtual async Task TestGetAssetPricesAsync(PublicAssetPricesContext context)
        {
            var p = IsType<IPublicPricesProvider>();
            if (p.Success)
                await GetAssetPricesAsync(p.Provider, context).ConfigureAwait(false);
        }

        public virtual async Task TestGetPricesAsync() { }
        public virtual async Task TestGetPricesAsync(PublicPricesContext context)
        {
            var p = IsType<IPublicPricesProvider>();
            if (p.Success)
                await GetPricesAsync(p.Provider, context).ConfigureAwait(false);
        }

        public virtual async Task TestGetAssetPairsAsync() { }
        public async Task TestGetAssetPairsAsync(AssetPairs requiredPairs)
        {
            var p = IsType<IAssetPairsProvider>();
            if (p.Success)
                await GetAssetPairsAsync(p.Provider, requiredPairs).ConfigureAwait(false);
        }

        public virtual async Task TestGetBalancesAsync()
        {
            var p = IsType<IBalanceProvider>();
            if (p.Success)
                await GetBalancesAsync(p.Provider).ConfigureAwait(false);
        }

        public virtual async Task TestGetAddressesAsync() { }
        public async Task TestGetAddressesAsync(WalletAddressContext context)
        {
            var p = IsType<IDepositProvider>();
            if (p.Success)
                await GetAddressesAsync(p.Provider, context).ConfigureAwait(false);
        }


        public virtual async Task TestGetAddressesForAssetAsync() { }
        public async Task TestGetAddressesForAssetAsync(WalletAddressAssetContext context)
        {
            var p = IsType<IDepositProvider>();
            if (p.Success)
                await GetAddressesForAssetAsync(p.Provider, context).ConfigureAwait(false);
        }

        public virtual async Task TestGetOrderBookAsync() { }
        public async Task TestGetOrderBookAsync(OrderBookContext context)
        {
            var p = IsType<IOrderBookProvider>();
            if (p.Success)
                await GetOrderBookAsync(p.Provider, context).ConfigureAwait(false);
        }

        public virtual async Task TestGetWithdrawalHistoryAsync() { }
        public async Task TestGetWithdrawalHistoryAsync(WithdrawalHistoryContext context)
        {
            var p = IsType<IWithdrawalHistoryProvider>();
            if (p.Success)
                await GetWithdrawalHistoryAsync(p.Provider, context).ConfigureAwait(false);
        }


        public virtual async Task TestPlaceWithdrawalAsync() { }
        public async Task TestPlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var p = IsType<IWithdrawalPlacementProvider>();
            if (p.Success)
                await PlaceWithdrawalAsync(p.Provider, context).ConfigureAwait(false);
        }

        public virtual async Task TestPlaceWithdrawalExtendedAsync() { }
        public async Task TestPlaceWithdrawalExtendedAsync(WithdrawalPlacementContextExtended context)
        {
            var p = IsType<IWithdrawalPlacementProviderExtended>();
            if (p.Success)
                await PlaceWithdrawalExtendedAsync(p.Provider, context).ConfigureAwait(false);
        }

        public virtual async Task TestCancelWithdrawalAsync() { }
        public async Task TestCancelWithdrawalAsync(WithdrawalCancelationContext context)
        {
            var p = IsType<IWithdrawalCancelationProvider>();
            if (p.Success)
                await CancelWithdrawalAsync(p.Provider, context).ConfigureAwait(false);
        }


        public virtual async Task TestConfirmWithdrawalAsync() { }
        public async Task TestConfirmWithdrawalAsync(WithdrawalConfirmationContext context)
        {
            var p = IsType<IWithdrawalConfirmationProvider>();
            if (p.Success)
                await ConfirmWithdrawalAsync(p.Provider, context).ConfigureAwait(false);
        }

        public virtual async Task TestGetVolumeAsync() { }
        public virtual async Task TestGetVolumeAsync(VolumeContext context)
        {
            var p = IsType<IAssetPairVolumeProvider>();
            if (p.Success)
                await GetVolumeAsync(p.Provider, context).ConfigureAwait(false);
        }

        #endregion

        #region Test methods


        private async Task TestApiAsync(INetworkProviderPrivate provider)
        {
            var ctx = new ApiPrivateTestContext(UserContext.Current.GetApiKey(provider));

            try
            {
                var r = await provider.TestPrivateApiAsync(ctx).ConfigureAwait(false);
                Assert.IsTrue(r);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Obsolete]
        protected OhlcContext OhlcContext { get; set; }

        private async Task GetOhlcAsync(IOhlcProvider provider, OhlcContext context)
        {
            if (context == null)
                return;

            try
            {
                var ohlc = await provider.GetOhlcAsync(context).ConfigureAwait(false);

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

        private async Task GetPriceAsync(IPublicPriceSuper provider, PublicPriceContext context, bool lessThan1)
        {
            if (context == null)
                return;

            MarketPrice c = null;

            try
            {
                if (provider is IPublicPriceProvider ipp)
                {
                    c = await ipp.GetPriceAsync(context).ConfigureAwait(false);

                    if (provider is IPublicPriceStatistics ipps)
                    {
                        // Only for single asset is supported

                        Assert.IsTrue(c.HasStatistics, $"Market price does not have statistics - {context.Pair}");
                        Trace.WriteLine($"Market price statistics for {context.Pair}:");

                        Trace.WriteLine($"Bid: {(c.PriceStatistics.HasHighestBid ? c.PriceStatistics.HighestBid.Display : "-")}");
                        Trace.WriteLine($"Ask: {(c.PriceStatistics.HasLowestAsk ? c.PriceStatistics.LowestAsk.Display : "-")}");
                        Trace.WriteLine($"Low: {(c.PriceStatistics.HasPrice24Low ? c.PriceStatistics.Price24Low.Display : "-")}");
                        Trace.WriteLine($"High: {(c.PriceStatistics.HasPrice24High ? c.PriceStatistics.Price24High.Display : "-")}");
                        Trace.WriteLine($"Volume 24h: {(c.PriceStatistics.HasVolume24 ? c.PriceStatistics.Volume24.ToString(CultureInfo.InvariantCulture) : "-")}");
                        Trace.WriteLine($"Quote volume 24h: {(c.PriceStatistics.HasVolume24Quote ? c.PriceStatistics.Volume24Quote.ToString(CultureInfo.InvariantCulture) : "-" )}\n");
                    }
                }
                else if (provider is IPublicPricesProvider ips)
                {
                    var r = await ips.GetPricesAsync(context).ConfigureAwait(false);
                    c = r?.MarketPrices?.FirstOrDefault();
                }

                Assert.IsTrue(c != null);
                Assert.IsTrue(c.QuoteAsset.Equals(context.Pair.Asset1));
                Assert.IsTrue(c.Price.Asset.Equals(context.Pair.Asset2));

                if (lessThan1)//checks if the pair is reversed (price-wise)
                    Assert.IsTrue(c.Price < 1, "Reverse check failed");
                else
                    Assert.IsTrue(c.Price > 1, "Reverse check failed");

                Trace.WriteLine($"Asset: {c.QuoteAsset}, Price: {c.Price.Display}");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Obsolete]
        protected PublicAssetPricesContext PublicAssetPricesContext { get; set; }

        private async Task GetAssetPricesAsync(IPublicAssetPricesProvider provider, PublicAssetPricesContext context)
        {
            if (context == null)
                return;

            try
            {
                var c = await provider.GetAssetPricesAsync(context).ConfigureAwait(false);

                Assert.IsNotNull(c);
                Assert.IsTrue(c.MarketPrices.Count == context.Assets.Count);

                Assert.IsFalse(c.MissedPairs.Any(), $"Provider did not return:{c.MissedPairs.Aggregate("", (s, pair) => s += $" {pair},").TrimEnd(',')}");

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

        [Obsolete]
        protected AssetPairs RequiredAssetPairs { get; set; }

        private async Task GetAssetPairsAsync(IAssetPairsProvider provider, AssetPairs requiredPairs)
        {
            var ctx = new NetworkProviderContext();

            try
            {
                var pairs = await provider.GetAssetPairsAsync(ctx).ConfigureAwait(false);

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

        [Obsolete]
        protected PublicPricesContext PublicPricesContext { get; set; }

        private async Task GetPricesAsync(IPublicPricesProvider provider, PublicPricesContext context)
        {
            if (context == null)
                return;

            try
            {
                var pairs = await provider.GetPricesAsync(context).ConfigureAwait(false);

                Assert.IsTrue(pairs != null);
                Assert.IsTrue(pairs.MarketPrices.Count > 0);

                Assert.IsFalse(pairs.MissedPairs.Any(), $"Provider did not return:{pairs.MissedPairs.Aggregate("", (s, pair) => s +=  $" {pair},").TrimEnd(',')}");

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

        private async Task GetBalancesAsync(IBalanceProvider provider)
        {
            var ctx = new NetworkProviderPrivateContext(UserContext.Current);

            try
            {
                var balances = await provider.GetBalancesAsync(ctx).ConfigureAwait(false);

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

        [Obsolete]
        protected WalletAddressContext WalletAddressContext { get; set; }

        private async Task GetAddressesAsync(IDepositProvider provider, WalletAddressContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = await provider.GetAddressesAsync(context).ConfigureAwait(false);

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

        [Obsolete]
        protected WalletAddressAssetContext WalletAddressAssetContext { get; set; }

        private async Task GetAddressesForAssetAsync(IDepositProvider provider, WalletAddressAssetContext context)
        {
            if(context == null)
                return;

            try
            {
                var r = await provider.GetAddressesForAssetAsync(context).ConfigureAwait(false);

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

        [Obsolete]
        protected OrderBookContext OrderBookContext { get; set; }

        private async Task GetOrderBookAsync(IOrderBookProvider provider, OrderBookContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = await provider.GetOrderBookAsync(context).ConfigureAwait(false);
                Assert.IsTrue(r != null);

                if (context.MaxRecordsCount.HasValue)
                    Assert.IsTrue(r.Count == context.MaxRecordsCount.Value);
                else
                    Assert.IsTrue(r.Count > 0);

                Trace.WriteLine($"Order book data ({r.Count(x => x.Type == OrderBookType.Ask)} asks, {r.Count(x => x.Type == OrderBookType.Bid)} bids): ");
                foreach (var obr in r)
                {
                    Trace.WriteLine($"{obr.Data.Time} | For {context.Pair.Asset1}: {obr.Type} {obr.Data.Price.Display}, {obr.Data.Volume} ");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Obsolete]
        protected WithdrawalHistoryContext WithdrawalHistoryContext { get; set; }

        private async Task GetWithdrawalHistoryAsync(IWithdrawalHistoryProvider provider, WithdrawalHistoryContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = await provider.GetWithdrawalHistoryAsync(context).ConfigureAwait(false);

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

        [Obsolete]
        protected WithdrawalPlacementContext WithdrawalPlacementContext { get; set; }

        private async Task PlaceWithdrawalAsync(IWithdrawalPlacementProvider provider, WithdrawalPlacementContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = await provider.PlaceWithdrawalAsync(context).ConfigureAwait(false);

                // Assert.IsTrue(r);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Obsolete]
        protected WithdrawalPlacementContextExtended WithdrawalPlacementContextExtended { get; set; }

        private async Task PlaceWithdrawalExtendedAsync(IWithdrawalPlacementProviderExtended provider, WithdrawalPlacementContextExtended context)
        {
            if (context == null)
                return;

            try
            {
                var r = await provider.PlaceWithdrawalAsync(context).ConfigureAwait(false);

                Assert.IsTrue(r != null);

                Trace.WriteLine($"Withdrawal request remote id: {r.WithdrawalRemoteId}");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Obsolete]
        protected WithdrawalCancelationContext WithdrawalCancelationContext { get; set; }

        private async Task CancelWithdrawalAsync(IWithdrawalCancelationProvider provider, WithdrawalCancelationContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = await provider.CancelWithdrawalAsync(context).ConfigureAwait(false);

                Assert.IsTrue(r != null);
                Assert.IsTrue(r.WithdrawalRemoteId.Equals(context.WithdrawalRemoteId), "Withdrawal ids don't match.");

                Trace.WriteLine($"Withdrawal request canceled, remote id is {r.WithdrawalRemoteId}");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Obsolete]
        protected WithdrawalConfirmationContext WithdrawalConfirmationContext { get; set; }

        private async Task ConfirmWithdrawalAsync(IWithdrawalConfirmationProvider provider, WithdrawalConfirmationContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = await provider.ConfirmWithdrawalAsync(context).ConfigureAwait(false);

                Assert.IsTrue(r != null);
                Assert.IsTrue(r.WithdrawalRemoteId.Equals(context.WithdrawalRemoteId), "Withdrawal ids don't match.");

                Trace.WriteLine($"Withdrawal request confirmed, remote id is {r.WithdrawalRemoteId}");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Obsolete]
        protected VolumeContext VolumeContext { get; set; }
        private async Task GetVolumeAsync(IAssetPairVolumeProvider provider, VolumeContext context)
        {
            if (context == null)
                return;

            try
            {
                var r = await provider.GetVolumeAsync(context).ConfigureAwait(false);

                Assert.IsTrue(r != null);
                Assert.IsTrue(r.Pair.Equals(context.Pair), $"Pairs don't match. Input is {context.Pair} and returned is {r.Pair}");

                Trace.WriteLine($"Period: {r.Period}, Pair: {r.Pair}, Volume: {r.Volume}");
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