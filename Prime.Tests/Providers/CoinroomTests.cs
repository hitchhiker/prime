using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Coinroom;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class CoinroomTests : ProviderDirectTestsBase
    {
        public CoinroomTests()
        {
            Provider = Networks.I.Providers.OfType<CoinroomProvider>().FirstProvider();
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
            "BTC_USD".ToAssetPairRaw(),
            "LTC_EUR".ToAssetPairRaw(),
            "LTC_BTC".ToAssetPairRaw()
        };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
        {
            "BTC_USD".ToAssetPairRaw(),
            "LTC_EUR".ToAssetPairRaw(),
            "LTC_BTC".ToAssetPairRaw()
        };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
