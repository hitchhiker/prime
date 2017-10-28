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

        public virtual async Task TestGetPairPriceAsync()
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

        public virtual async Task TestGetPairsPricesAsync()
        {
            var p = IsType<IPublicPricesProvider>();
            if (p.Success)
                await GetPairsPricesAsync(p.Provider);
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

                Trace.WriteLine($"Latest price for {c.QuoteAsset}: {c.Price.Display}");
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

                Assert.IsTrue(c != null);
                Assert.IsTrue(c.FirstOrDefault()?.QuoteAsset?.Equals(PublicAssetPricesContext.QuoteAsset) == true);
                Assert.IsTrue(c.Count == PublicAssetPricesContext.Assets.Count);

                Trace.WriteLine($"Latest prices for {PublicAssetPricesContext.QuoteAsset}:");
                foreach (var i in c)
                {
                    Trace.WriteLine(i.Price.Display);
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
                    foreach (var assetPair in pairs)
                    {
                        Assert.IsTrue(RequiredAssetPairs.Contains(assetPair), $"Provider didn't return required {assetPair} pair.");
                    }
                }

                Trace.WriteLine("Asset pairs:");
                foreach (var pair in pairs)
                {
                    Trace.WriteLine($"{pair.Asset1}-{pair.Asset2}");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected PublicPricesContext PublicPricesContext { get; set; }

        private async Task GetPairsPricesAsync(IPublicPricesProvider provider)
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
                Assert.IsTrue(pairs.Count > 0);

                Trace.WriteLine("Latest prices:");
                foreach (var pair in pairs)
                {
                    Trace.WriteLine($"{pair.QuoteAsset}: {pair.Price.Display}");
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