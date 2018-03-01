using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Yobit;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class YobitTests : ProviderDirectTestsBase
    {
        public YobitTests()
        {
            Provider = Networks.I.Providers.OfType<YobitProvider>().FirstProvider();
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
                "ltc_btc".ToAssetPairRaw(),
                "ppc_btc".ToAssetPairRaw(),
                "dash_btc".ToAssetPairRaw(),
                "doge_btc".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ltc_btc".ToAssetPairRaw(),
                "ppc_btc".ToAssetPairRaw(),
                "dash_btc".ToAssetPairRaw(),
                "doge_btc".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("ltc_btc".ToAssetPairRaw(), true);
        }

        [TestMethod]
        public override void TestPlaceWithdrawal()
        {
            // TODO: SC: Not tested with real money
            base.TestPlaceWithdrawal(new WalletAddress("1234"), new Money(22, Asset.Btc));
        }

        [TestMethod]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: SC: Not tested with real money
            var orderId = "21109502";
            base.TestGetTradeOrderStatus(orderId, "btc_usd".ToAssetPairRaw());
        }

        [TestMethod]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money
            base.TestPlaceOrderLimit("btc_usd".ToAssetPairRaw(), false, 1, new Money(1m, Asset.Usd));
        }
    }
}
