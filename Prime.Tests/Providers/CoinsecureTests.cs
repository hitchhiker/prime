using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Coinsecure;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class CoinsecureTests : ProviderDirectTestsBase
    {
        public CoinsecureTests()
        {
            Provider = Networks.I.Providers.OfType<CoinsecureProvider>().FirstProvider();
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
                "BTC_INR".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_INR".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
