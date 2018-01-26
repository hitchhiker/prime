using Prime.Plugins.Services.MixCoins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class MixCoinsTests : ProviderDirectTestsBase
    {
        public MixCoinsTests()
        {
            Provider = Networks.I.Providers.OfType<MixCoinsProvider>().FirstProvider();
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
                "eth_btc".ToAssetPairRaw(),
                "bcc_btc".ToAssetPairRaw(),
                "lsk_btc".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_usd".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw(),
                "bcc_btc".ToAssetPairRaw(),
                "lsk_btc".ToAssetPairRaw()
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
