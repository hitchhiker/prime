using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.EthexIndia;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class EthexIndiaTests : ProviderDirectTestsBase
    {
        public EthexIndiaTests()
        {
            Provider = Networks.I.Providers.OfType<EthexIndiaProvider>().FirstProvider();
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
                "ETH_INR".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ETH_INR".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
