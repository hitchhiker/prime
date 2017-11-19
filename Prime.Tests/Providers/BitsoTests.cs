using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Bitso;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class BitsoTests : ProviderDirectTestsBase
    {
        public BitsoTests()
        {
            Provider = Networks.I.Providers.OfType<BitsoProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "eth_btc".ToAssetPairRaw()
            };
            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "eth_mxn".ToAssetPairRaw(),
                "xrp_btc".ToAssetPairRaw(),
                "xrp_mxn".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw(),
                "bch_btc".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
