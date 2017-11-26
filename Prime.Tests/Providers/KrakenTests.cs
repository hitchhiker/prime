using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Plugins.Services.Kraken;
using AssetPair = Prime.Common.AssetPair;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class KrakenTests : ProviderDirectTestsBase
    {
        public KrakenTests()
        {
            Provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();
        }

        [TestMethod]
        public void TestAllAssetPairsPrices()
        {
            var pairs = AsyncContext.Run(() => (IAssetPairsProvider) Provider).GetAssetPairsAsync(new NetworkProviderContext()).Result;

            var prices = AsyncContext.Run(() => (IPublicPricingProvider) Provider).GetPricingAsync(new PublicPricesContext(pairs.ToList())).Result;

            Assert.IsTrue(!prices.MissedPairs.Any());
            Assert.IsTrue(pairs.Count == prices.MarketPrices.Count);
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "ETC_ETH".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [TestMethod]
        public override void TestGetAddresses()
        {
            // BUG: EFunding:Too many addresses. Should investigate that.

            var context = new WalletAddressContext(UserContext.Current);
            base.TestGetAddresses(context);
        }

        [TestMethod]
        public override void TestGetAddressesForAsset()
        {
            // BUG: EFunding:Too many addresses. Should investigate that.
            var context = new WalletAddressAssetContext("MLN".ToAssetRaw(), UserContext.Current);
            base.TestGetAddressesForAsset(context);
        }

        [TestMethod]
        public override void TestGetOhlc()
        {
            var context = new OhlcContext("ETC_ETH".ToAssetPairRaw(), TimeResolution.Minute, TimeRange.EveryDayTillNow);
            base.TestGetOhlc(context);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var pairs = new AssetPairs()
            {
                "ETC_ETH".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
            };

            base.TestGetAssetPairs(pairs);
        }

        [TestMethod]
        public override void TestApi()
        {
            base.TestApi();
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            var context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()));
            base.TestGetOrderBook(context);

            context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()), 100);
            base.TestGetOrderBook(context);
        }
    }
}
