using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.OkCoin;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class OkCoinTests : ProviderDirectTestsBase
    {
        public OkCoinTests()
        {
            Provider = Networks.I.Providers.OfType<OkCoinProvider>().FirstProvider();
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
                "btc_usd".ToAssetPairRaw(),
                "ltc_usd".ToAssetPairRaw(),
                "eth_usd".ToAssetPairRaw(),
                "etc_usd".ToAssetPairRaw(),
                "bch_usd".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_usd".ToAssetPairRaw(),
                "ltc_usd".ToAssetPairRaw(),
                "eth_usd".ToAssetPairRaw(),
                "etc_usd".ToAssetPairRaw(),
                "bch_usd".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("btc_usd".ToAssetPairRaw(), false);
        }
    }
}
