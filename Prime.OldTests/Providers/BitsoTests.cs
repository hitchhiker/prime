using System.Collections.Generic;
using System.Linq;
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
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "eth_btc".ToAssetPairRaw(),
                "eth_mxn".ToAssetPairRaw(),
                "xrp_btc".ToAssetPairRaw(),
                "xrp_mxn".ToAssetPairRaw(),
                "bch_btc".ToAssetPairRaw()
            };
            base.TestGetPricing(pairs, true);
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

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("eth_btc".ToAssetPairRaw(), true);
        }
    }
}
