using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.StocksExchange;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class StocksExchangeTests : ProviderDirectTestsBase
    {
        public StocksExchangeTests()
        {
            Provider = Networks.I.Providers.OfType<StocksExchangeProvider>().FirstProvider();
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
                "EAG_BTC".ToAssetPairRaw(),
                "NXT_BTC".ToAssetPairRaw(),
                "STEX_NXT".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "EAG_BTC".ToAssetPairRaw(),
                "NXT_BTC".ToAssetPairRaw(),
                "STEX_NXT".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
