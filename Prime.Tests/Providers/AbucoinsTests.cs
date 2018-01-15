using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Abucoins;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class AbucoinsTests : ProviderDirectTestsBase
    {
        public AbucoinsTests()
        {
            Provider = Networks.I.Providers.OfType<AbucoinsProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETC_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETC_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetVolume()
        {
            var pairs = new List<AssetPair>()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETC_BTC".ToAssetPairRaw()
            };

            base.TestGetVolume(pairs, false);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("BTC_PLN".ToAssetPairRaw(), false);
        }
    }
}
