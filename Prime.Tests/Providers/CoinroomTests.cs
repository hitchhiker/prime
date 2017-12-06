using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Coinroom;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class CoinroomTests : ProviderDirectTestsBase
    {
        public CoinroomTests()
        {
            Provider = Networks.I.Providers.OfType<CoinroomProvider>().FirstProvider();
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
            "BTC_USD".ToAssetPairRaw(),
            "LTC_EUR".ToAssetPairRaw(),
            "BTC_EUR".ToAssetPairRaw()
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
            "BTC_EUR".ToAssetPairRaw()
        };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
