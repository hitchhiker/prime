using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Kraken;
using Prime.Plugins.Services.Poloniex;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class PoloniexTests : ProviderDirectTestsBase
    {
        public PoloniexTests()
        {
            Provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "ETH_ETC".ToAssetPairRaw(),
                "BTC_ETC".ToAssetPairRaw(),
                "USDT_BTC".ToAssetPairRaw(),
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestApi()
        {
            base.TestApi();
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            var context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "NXT".ToAssetRaw()));
            TestGetOrderBook(context);

            context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "NXT".ToAssetRaw()), 100);
            TestGetOrderBook(context);
        }

        [TestMethod]
        public override void TestGetAddresses()
        {
            var context = new WalletAddressContext(UserContext.Current);
            TestGetAddresses(context);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "ETH_ETC".ToAssetPairRaw(),
                "BTC_ETC".ToAssetPairRaw(),
                "USDT_BTC".ToAssetPairRaw(),
            };

            TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            base.TestGetAddressesForAsset(context);
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [TestMethod]
        public override void TestGetOhlc()
        {
            // BUG: supports only 1 day.
            var context = new OhlcContext(new AssetPair("BTC", "LTC"), TimeResolution.Day,
                new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Hour));

            base.TestGetOhlc(context);
        }
    }
}
