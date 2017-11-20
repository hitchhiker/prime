using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Core;
using Prime.Plugins.Services.Binance;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BinanceTests : ProviderDirectTestsBase 
    {
        public BinanceTests()
        {
            Provider = Networks.I.Providers.OfType<BinanceProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "LTC_BTC".ToAssetPairRaw(),
                "BNT_BTC".ToAssetPairRaw(),
                "SALT_ETH".ToAssetPairRaw(),
                "SALT_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "NEO_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "NEO_ETH".ToAssetPairRaw(),
                "IOTA_BTC".ToAssetPairRaw(),
                "ETC_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            var context = new OrderBookContext(new AssetPair("BNT".ToAssetRaw(), "BTC".ToAssetRaw()));
            base.TestGetOrderBook(context);

            context = new OrderBookContext(new AssetPair("BNT".ToAssetRaw(), "BTC".ToAssetRaw()), 20);
            base.TestGetOrderBook(context);
        }

        [TestMethod]
        public override void TestApi()
        {
            base.TestApi();
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [TestMethod]
        public override void TestGetOhlc()
        {
            var context = new OhlcContext(new AssetPair("BNT", "BTC"), TimeResolution.Minute,
                new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Minute));
            base.TestGetOhlc(context);
        }

        [TestMethod]
        public override void TestGetVolume()
        {
            var context = new PublicVolumeContext("BNT_BTC".ToAssetPairRaw());

            base.TestGetVolume(context);
        }
    }
}
