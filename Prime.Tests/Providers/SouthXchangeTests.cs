using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.SouthXchange;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class SouthXchangeTests : ProviderDirectTestsBase
    {
        public SouthXchangeTests()
        {
            Provider = Networks.I.Providers.OfType<SouthXchangeProvider>().FirstProvider();
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
                "DOGE_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BTG_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "DOGE_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BTG_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("DOGE_BTC".ToAssetPairRaw(), true);
        }
    }
}
