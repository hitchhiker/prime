using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Liqui;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class LiquiTests : ProviderDirectTestsBase
    {
        public LiquiTests()
        {
            Provider = Networks.I.Providers.OfType<LiquiProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [TestMethod]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: AY: test using real money - Liqui.
            base.TestGetTradeOrderStatus("98217034");
        }

        [TestMethod]
        public override void TestPlaceOrderLimit()
        {
            // TODO: AY: test using real money - Liqui.
            base.TestPlaceOrderLimit("ETH_USDT".ToAssetPairRaw(), true, 1m, new Money(1, Asset.UsdT));
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
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ltc_btc".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw(),
                "ans_btc".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("eth_btc".ToAssetPairRaw(), true);
        }
    }
}
