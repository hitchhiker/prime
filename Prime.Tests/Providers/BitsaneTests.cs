using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Bitsane;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BitsaneTests : ProviderDirectTestsBase
    {
        public BitsaneTests()
        {
            Provider = Networks.I.Providers.OfType<BitsaneProvider>().FirstProvider();
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
                "BTC_USD".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
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
                "BCH_BTC".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
        }
    }
}
