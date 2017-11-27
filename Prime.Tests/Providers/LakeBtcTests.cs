using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.LakeBtc;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class LakeBtcTests : ProviderDirectTestsBase
    {
        public LakeBtcTests()
        {
            Provider = Networks.I.Providers.OfType<LakeBtcProvider>().FirstProvider();
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
                "btc_usd".ToAssetPairRaw(),
                "btc_gbp".ToAssetPairRaw(),
                "btc_cad".ToAssetPairRaw(),
                "btc_chf".ToAssetPairRaw(),
                "btc_ngn".ToAssetPairRaw(),
                "gbp_usd".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_usd".ToAssetPairRaw(),
                "btc_gbp".ToAssetPairRaw(),
                "btc_cad".ToAssetPairRaw(),
                "btc_chf".ToAssetPairRaw(),
                "btc_ngn".ToAssetPairRaw(),
                "gbp_usd".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
