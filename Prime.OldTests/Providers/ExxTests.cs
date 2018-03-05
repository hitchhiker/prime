using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Exx;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class ExxTests : ProviderDirectTestsBase
    {
        public ExxTests()
        {
            Provider = Networks.I.Providers.OfType<ExxProvider>().FirstProvider();
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
                "bts_eth".ToAssetPairRaw(),
                "BTM_ETH".ToAssetPairRaw(),
                "EOS_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "bts_eth".ToAssetPairRaw(),
                "BTM_ETH".ToAssetPairRaw(),
                "EOS_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("EOS_BTC".ToAssetPairRaw(), true);
        }
    }
}
