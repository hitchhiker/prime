using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Gate;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class GateTests : ProviderDirectTestsBase
    {
        public GateTests()
        {
            Provider = Networks.I.Providers.OfType<GateProvider>().FirstProvider();
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
                "etc_eth".ToAssetPairRaw(),
                "pay_btc".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ltc_btc".ToAssetPairRaw(),
                "etc_eth".ToAssetPairRaw(),
                "pay_btc".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetVolume()
        {
            var pairs = new List<AssetPair>()
            {
                "ltc_btc".ToAssetPairRaw(),
                "etc_eth".ToAssetPairRaw(),
                "pay_btc".ToAssetPairRaw()
            };

            base.TestGetVolume(pairs, false);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("ltc_btc".ToAssetPairRaw(), true);
        }
    }
}
