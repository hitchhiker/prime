using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Bisq;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class BisqTests : ProviderDirectTestsBase
    {
        public BisqTests()
        {
            Provider = Networks.I.Providers.OfType<BisqProvider>().FirstProvider();
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
                "btc_eur".ToAssetPairRaw(),
                "btc_nok".ToAssetPairRaw(),
                "btc_aud".ToAssetPairRaw(),
                "etc_btc".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_eur".ToAssetPairRaw(),
                "btc_nok".ToAssetPairRaw(),
                "btc_aud".ToAssetPairRaw(),
                "etc_btc".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
