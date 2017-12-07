using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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
            Provider = Networks.I.Providers.OfType<TidexProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [TestMethod]
        public void TestGetOrderStatus()
        {
            var tradingProvider = Provider as IOrderLimitProvider;
            var context = new RemoteIdContext(UserContext.Current, "12351");

            var r = tradingProvider.GetOrderStatusAsync(context);

            Assert.IsTrue(r != null);
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
    }
}
