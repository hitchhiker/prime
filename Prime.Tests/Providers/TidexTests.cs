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
            Provider = Networks.I.Providers.OfType<TidexProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [TestMethod]
        public override void TestGetTradeOrderStatus()
        {
            base.TestGetTradeOrderStatus("12351");
        }

        [TestMethod]
        public void TestPlaceTradeOrder()
        {
            var tradingProvider = Provider as IOrderLimitProvider;

            var context = new PlaceOrderLimitContext(UserContext.Current, "LTC_BTC".ToAssetPairRaw(), false, 1, new Money(1, Asset.Btc));

            var r = tradingProvider.PlaceOrderLimitAsync(context);
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
