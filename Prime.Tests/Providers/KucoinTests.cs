using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Kucoin;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class KucoinTests : ProviderDirectTestsBase
    {
        public KucoinTests()
        {
            Provider = Networks.I.Providers.OfType<KucoinProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [TestMethod]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "KCS_BTC".ToAssetPairRaw(),
                "KNC_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "KCS_BTC".ToAssetPairRaw(),
                "KNC_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("KCS_BTC".ToAssetPairRaw(), true);
        }
    }
}
