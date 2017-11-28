using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Gemini;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class GeminiTests : ProviderDirectTestsBase
    {
        public GeminiTests()
        {
            Provider = Networks.I.Providers.OfType<GeminiProvider>().FirstProvider();
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
                "ETH_USD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
