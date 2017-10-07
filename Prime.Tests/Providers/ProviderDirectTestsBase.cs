using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Core;
using Prime.Core.Exchange;

namespace Prime.Tests
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
            var p = IsType<IPublicPriceProvider>();
            if (p.Success)
                await GetLatestPriceAsync(p.Provider);
        }

        public virtual async Task TestGetLatestPricesAsync()
        {
            var p = IsType<IPublicPricesProvider>();
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

        public virtual async Task TestGetOrderBookLiveAsync()
        {
            var p = IsType<IOrderBookProvider>();
            if (p.Success)
                await GetOrderBookLiveAsync(p.Provider);
        }

        public virtual async Task TestGetOrderBookHistoryAsync()
        {
            var p = IsType<IOrderBookProvider>();
            if (p.Success)
                await GetOrderBookHistoryAsync(p.Provider);
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

        private async Task GetOhlcAsync(IOhlcProvider provider)
        {
            var ohlcContext = new OhlcContext(new AssetPair("BTC", "USD"), TimeResolution.Minute, new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Minute), null);

            try
            {
                var ohlc = await provider.GetOhlcAsync(ohlcContext);

                Assert.IsTrue(ohlc != null && ohlc.Count > 0);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected PublicPriceContext PublicPriceContext { get; set; }

        private async Task GetLatestPriceAsync(IPublicPriceProvider provider)
        {
            if (PublicPriceContext == null)
            {
                PublicPriceContext = new PublicPriceContext(new AssetPair("BTC", "USD"));
            }
            
            try
            {
                var c = await provider.GetLatestPriceAsync(PublicPriceContext);

                Assert.IsTrue(c != null);
                //Assert.IsTrue(c.BaseAsset.Equals(PublicPriceContext.Pair.Asset1));
                //Assert.IsTrue(c.Price.Asset.Equals(PublicPriceContext.Pair.Asset2));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected PublicPricesContext PublicPricesContext { get; set; }

        private async Task GetLatestPricesAsync(IPublicPricesProvider provider)
        {
            if (PublicPricesContext == null)
            {
                PublicPricesContext = new PublicPricesContext(Asset.Btc, new List<Asset>()
                {
                    "USD".ToAssetRaw(),
                    "EUR".ToAssetRaw()
                });
            }

            try
            {
                var c = await provider.GetLatestPricesAsync(PublicPricesContext);

                Assert.IsTrue(c != null);
                Assert.IsTrue(c.BaseAsset.Equals(PublicPricesContext.BaseAsset));
                Assert.IsTrue(c.Prices.Count == PublicPricesContext.Assets.Count);
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
                var addresses = await provider.GetAddressesAsync(ctx);

                Assert.IsTrue(addresses != null);
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
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private async Task GetOrderBookLiveAsync(IOrderBookProvider provider)
        {
            var ctx = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()), 10);

            try
            {
                var r = await provider.GetOrderBookLive(ctx);
                Assert.IsTrue(r != null);
                Assert.IsTrue(r.Count == 1);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private async Task GetOrderBookHistoryAsync(IOrderBookProvider provider)
        {
            var ctx = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()), 100);

            try
            {
                var r = await provider.GetOrderBookHistory(ctx);
                Assert.IsTrue(r != null);
                Assert.IsTrue(r.Count == ctx.Depth);
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