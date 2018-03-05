using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BtcMarkets;
using Prime.Tests.Providers;

namespace Prime.Tests
{
    [TestClass()]
    public class BtcMarketsTests : ProviderDirectTestsBase
    {
        public BtcMarketsTests()
        {
            Provider = Networks.I.Providers.OfType<BtcMarketsProvider>().FirstProvider();
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
                "BCH_AUD".ToAssetPairRaw(),
                "BTC_AUD".ToAssetPairRaw(),
                "ETH_AUD".ToAssetPairRaw(),
                "LTC_AUD".ToAssetPairRaw(),
                "XRP_AUD".ToAssetPairRaw(),
                "ETC_AUD".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BCH_AUD".ToAssetPairRaw(),
                "BTC_AUD".ToAssetPairRaw(),
                "ETH_AUD".ToAssetPairRaw(),
                "LTC_AUD".ToAssetPairRaw(),
                "XRP_AUD".ToAssetPairRaw(),
                "ETC_AUD".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }


        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("BTC_AUD".ToAssetPairRaw(), false);
        }
    }
}
