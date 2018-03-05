using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BrightonPeak;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BrightonPeakTests : ProviderDirectTestsBase
    {
        public BrightonPeakTests()
        {
            Provider = Networks.I.Providers.OfType<BrightonPeakProvider>().FirstProvider();
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
                "BTC_AUD".ToAssetPairRaw(),
                "LTC_AUD".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_AUD".ToAssetPairRaw(),
                "LTC_AUD".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }


        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("BTC_AUD".ToAssetPairRaw(), false);
        }
    }
}
