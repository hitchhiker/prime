using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Luno;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class LunoTests : ProviderDirectTestsBase
    {
        public LunoTests()
        {
            Provider = Networks.I.Providers.OfType<LunoProvider>().FirstProvider();
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
                "XBT_MYR".ToAssetPairRaw(),
                "XBT_IDR".ToAssetPairRaw(),
                "XBT_NGN".ToAssetPairRaw(),
                "XBT_ZAR".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "XBT_MYR".ToAssetPairRaw(),
                "XBT_IDR".ToAssetPairRaw(),
                "XBT_NGN".ToAssetPairRaw(),
                "XBT_ZAR".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
