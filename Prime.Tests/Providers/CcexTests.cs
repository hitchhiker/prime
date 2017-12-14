using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Ccex;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class CcexTests : ProviderDirectTestsBase
    {
        public CcexTests()
        {
            Provider = Networks.I.Providers.OfType<CcexProvider>().FirstProvider();
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
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "ZNY_DOGE".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "ZNY_DOGE".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
