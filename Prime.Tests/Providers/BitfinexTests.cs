using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Bitfinex;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class BitfinexTests : ProviderDirectTestsBase
    {
        public BitfinexTests()
        {
            Provider = Networks.I.Providers.OfType<BitfinexProvider>().FirstProvider();
        }

        #region Private

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
        public override void TestPlaceOrderLimit()
        {
            base.TestPlaceOrderLimit("XRP_USD".ToAssetPairRaw(), false, 34m, new Money(100000m, "USD".ToAssetRaw()));
        }

        [TestMethod]
        public override void TestGetTradeOrderStatus()
        {
            base.TestGetTradeOrderStatus("6313352603");
        }

        [TestMethod]
        public override void TestPlaceWithdrawal()
        {
            // TODO: AY: Bitfinex - test with real money.
            var context = new WithdrawalPlacementContext(UserContext.Current)
            {
                Address = new WalletAddress("6a51d6a5wda6w5d1"),
                Amount = new Money(10000, "UAH".ToAssetRaw()),
                Description = "DESC"
            };

            base.TestPlaceWithdrawal(context);
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
                "BTC_USD".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
        }

        #endregion
    }
}
