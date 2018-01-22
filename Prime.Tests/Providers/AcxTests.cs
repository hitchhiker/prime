using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Acx;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class AcxTests : ProviderDirectTestsBase
    {
        public AcxTests()
        {
            Provider = Networks.I.Providers.OfType<AcxProvider>().FirstProvider();
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
                "BCH_AUD".ToAssetPairRaw(),
                "ETH_AUD".ToAssetPairRaw(),
                "HSR_AUD".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_AUD".ToAssetPairRaw(),
                "BCH_AUD".ToAssetPairRaw(),
                "ETH_AUD".ToAssetPairRaw(),
                "HSR_AUD".ToAssetPairRaw()
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
