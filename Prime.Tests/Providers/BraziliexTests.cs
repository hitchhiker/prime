using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Braziliex;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BraziliexTests : ProviderDirectTestsBase
    {
        public BraziliexTests()
        {
            Provider = Networks.I.Providers.OfType<BraziliexProvider>().FirstProvider();
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
                "ltc_btc".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ltc_btc".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
