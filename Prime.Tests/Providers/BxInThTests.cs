using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BxInTh;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class BxInThTests : ProviderDirectTestsBase
    {
        public BxInThTests()
        {
            Provider = Networks.I.Providers.OfType<BxInThProvider>().FirstProvider();
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
                "BTC_DOG".ToAssetPairRaw(),
                "BTC_PPC".ToAssetPairRaw(),
                "BTC_XPM".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_DOG".ToAssetPairRaw(),
                "BTC_PPC".ToAssetPairRaw(),
                "BTC_XPM".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
