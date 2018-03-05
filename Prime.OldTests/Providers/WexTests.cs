using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Wex;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class WexTests : ProviderDirectTestsBase
    {
        public WexTests()
        {
            Provider = Networks.I.Providers.OfType<WexProvider>().FirstProvider();
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
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [TestMethod]
        public override void TestGetTradeOrderStatus()
        {
            base.TestGetTradeOrderStatus("98217034");
        }

        [TestMethod]
        public override void TestPlaceOrderLimit()
        {
            // TODO: AY: Wex - test with real money.
            base.TestPlaceOrderLimit("BTC_USD".ToAssetPairRaw(), true, 1m, new Money(1, Asset.Usd));
        }

        [TestMethod]
        public override void TestPlaceWithdrawal()
        {
            // TODO: AY: Wex - test with real money.
            base.TestPlaceWithdrawal(new WalletAddress("testaddress"), new Money(1, Asset.Usd));
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "btc_eur".ToAssetPairRaw(),
                "btc_usd".ToAssetPairRaw(),
                "ltc_usd".ToAssetPairRaw(),
                "ltc_eur".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_eur".ToAssetPairRaw(),
                "btc_usd".ToAssetPairRaw(),
                "ltc_usd".ToAssetPairRaw(),
                "ltc_eur".ToAssetPairRaw()
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
