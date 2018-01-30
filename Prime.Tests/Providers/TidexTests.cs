using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Tidex;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class TidexTests : ProviderDirectTestsBase
    {
        public TidexTests()
        {
            Provider = Networks.I.Providers.OfType<TidexProviderTiLiWe>().FirstProvider();
        }

        #region Private

        [TestMethod]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [TestMethod]
        public override void TestGetTradeOrderStatus()
        {
            base.TestGetTradeOrderStatus("98217034");
        }

        [TestMethod]
        public override void TestPlaceOrderLimit()
        {
            base.TestPlaceOrderLimit("ETH_USDT".ToAssetPairRaw(), true, 1m, new Money(1, Asset.UsdT));
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        #endregion

        #region Public

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
                "eth_btc".ToAssetPairRaw(),
                "dash_btc".ToAssetPairRaw(),
                "doge_btc".ToAssetPairRaw(),
                "bts_btc".ToAssetPairRaw(),
                "waves_btc".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "eth_btc".ToAssetPairRaw(),
                "dash_btc".ToAssetPairRaw(),
                "doge_btc".ToAssetPairRaw(),
                "bts_btc".ToAssetPairRaw(),
                "waves_btc".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("eth_btc".ToAssetPairRaw(), true);
        }

        #endregion
    }
}
