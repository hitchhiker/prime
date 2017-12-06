using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Bit2C;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class Bit2CTests : ProviderDirectTestsBase
    {
        public Bit2CTests()
        {
            Provider = Networks.I.Providers.OfType<Bit2CProvider>().FirstProvider();
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
                "BTC_NIS".ToAssetPairRaw(),
                "LTC_NIS".ToAssetPairRaw(),
                "BCH_NIS".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_NIS".ToAssetPairRaw(),
                "LTC_NIS".ToAssetPairRaw(),
                "BCH_NIS".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
