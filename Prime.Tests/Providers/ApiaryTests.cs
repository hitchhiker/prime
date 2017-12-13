using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Apiary;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class ApiaryTests : ProviderDirectTestsBase
    {
        public ApiaryTests()
        {
            Provider = Networks.I.Providers.OfType<ApiaryProvider>().FirstProvider();
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
                "eth_usd".ToAssetPairRaw(),
                "eth_eur".ToAssetPairRaw(),
                "btc_gbp".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "eth_usd".ToAssetPairRaw(),
                "eth_eur".ToAssetPairRaw(),
                "btc_gbp".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
