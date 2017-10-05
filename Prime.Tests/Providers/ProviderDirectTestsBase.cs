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

        #endregion

        #region Test methods


        public virtual async Task TestApiAsync(INetworkProviderPrivate provider)
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

        public virtual async Task GetOhlcAsync(IOhlcProvider provider)
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

        public virtual async Task GetLatestPriceAsync(IPublicPriceProvider provider)
        {
            var ctx = new PublicPriceContext(new AssetPair("BTC", "USD"));

            try
            {
                var c = await provider.GetLatestPriceAsync(ctx);

                Assert.IsTrue(c != null);
                Assert.IsTrue(c.BaseAsset.Equals(ctx.Pair.Asset1));
                Assert.IsTrue(c.Price.Asset.Equals(ctx.Pair.Asset2));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private async Task GetLatestPricesAsync(IPublicPricesProvider provider)
        {
            var ctx = new PublicPricesContext(Asset.Btc, new List<Asset>()
            {
                "USD".ToAssetRaw(),
                "EUR".ToAssetRaw()
            });

            try
            {
                var c = await provider.GetLatestPricesAsync(ctx);

                Assert.IsTrue(c != null);
                Assert.IsTrue(c.BaseAsset.Equals(ctx.BaseAsset));
                Assert.IsTrue(c.Prices.Count == ctx.Assets.Count);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }


        public virtual async Task GetAssetPairsAsync(IExchangeProvider provider)
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

        public virtual async Task GetBalancesAsync(IWalletService provider)
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

        public virtual async Task GetAddressesAsync(IDepositService provider)
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

        public virtual async Task GetAddressesForAssetAsync(IDepositService provider)
        {
            var asset = "BTC".ToAsset(provider);

            var ctx = new WalletAddressAssetContext(asset, false, UserContext.Current);

            try
            {
                var r = await provider.GetAddressesForAssetAsync(ctx);
                Assert.IsTrue(r != null);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        public virtual async Task GetOrderBookLiveAsync(IOrderBookProvider provider)
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