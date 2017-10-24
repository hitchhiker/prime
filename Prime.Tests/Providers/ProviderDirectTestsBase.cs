using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Common.Exchange;

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

        public virtual async Task TestGetLatestPriceAsync()
        {
            var p = IsType<IPublicPairPriceProvider>();
            if (p.Success)
                await GetLatestPriceAsync(p.Provider);
        }

        public virtual async Task TestGetLatestPricesAsync()
        {
            var p = IsType<IPublicAssetPricesProvider>();
            if (p.Success)
                await GetLatestPricesAsync(p.Provider);
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

        protected PublicPairPriceContext PublicPairPriceContext { get; set; }

        private async Task GetLatestPriceAsync(IPublicPairPriceProvider provider)
        {
            if (PublicPairPriceContext == null)
            {
                PublicPairPriceContext = new PublicPairPriceContext(new AssetPair("BTC", "USD"));
            }

            try
            {
                var c = await provider.GetPairPriceAsync(PublicPairPriceContext);

                Assert.IsTrue(c != null);
                Assert.IsTrue(c.BaseAsset.Equals(PublicPairPriceContext.Pair.Asset1));
                Assert.IsTrue(c.Price.Asset.Equals(PublicPairPriceContext.Pair.Asset2));

                Trace.WriteLine($"Latest price for {c.BaseAsset}: {c.Price.Display}");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected PublicAssetPricesContext PublicAssetPricesContext { get; set; }

        private async Task GetLatestPricesAsync(IPublicAssetPricesProvider provider)
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

                Assert.IsTrue(c != null);
                Assert.IsTrue(c.BaseAsset.Equals(PublicAssetPricesContext.QuoteAsset));
                Assert.IsTrue(c.Prices.Count == PublicAssetPricesContext.Assets.Count);

                Trace.WriteLine($"Latest prices for {c.BaseAsset}:");
                foreach (var latestPrice in c.Prices)
                {
                    Trace.WriteLine(latestPrice.Display);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private async Task GetAssetPairsAsync(IExchangeProvider provider)
        {
            var ctx = new NetworkProviderContext();

            try
            {
                var pairs = await provider.GetAssetPairs(ctx);

                Assert.IsTrue(pairs != null);
                Assert.IsTrue(pairs.Count > 0);
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

        private async Task GetAddressesAsync(IDepositService provider)
        {
            var ctx = new WalletAddressContext(false, UserContext.Current);

            try
            {
                var r = await provider.GetAddressesAsync(ctx);

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
                WalletAddressAssetContext = new WalletAddressAssetContext("BTC".ToAsset(provider), false, UserContext.Current);
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

                if(OrderBookContext.MaxRecordsCount.HasValue)
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