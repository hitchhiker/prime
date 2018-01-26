using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Core;
using Prime.Plugins.Services.Binance;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BinanceTests : ProviderDirectTestsBase 
    {
        public BinanceTests()
        {
            Provider = Networks.I.Providers.OfType<BinanceProvider>().FirstProvider();
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
            base.TestPlaceOrderLimit("XRP_BTC".ToAssetPairRaw(), false, 1, new Money(1m, Asset.Btc));
        }

        [TestMethod]
        public override void TestGetTradeOrderStatus()
        {
            var orderId = "21109502";
            base.TestGetTradeOrderStatus(orderId, "XRP_BTC".ToAssetPairRaw());
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
                "LTC_BTC".ToAssetPairRaw(),
                "BNT_BTC".ToAssetPairRaw(),
                "SALT_ETH".ToAssetPairRaw(),
                "SALT_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "NEO_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "NEO_ETH".ToAssetPairRaw(),
                "IOTA_BTC".ToAssetPairRaw(),
                "ETC_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("BTC_USDT".ToAssetPairRaw(), false, 50);
        }

        [TestMethod]
        public override void TestGetOhlc()
        {
            var context = new OhlcContext("BTC_USDT".ToAssetPairRaw(), TimeResolution.Minute,
                new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Minute));
            base.TestGetOhlc(context);
        }

        [TestMethod]
        public override void TestPlaceWithdrawal()
        {
            base.TestPlaceWithdrawal(new WalletAddress("rLW9gnQo7BQhU6igk5keqYnH3TVrCxGRzm"), new Money(22, Asset.Xrp), "3299088538");
        }

        public async Task TestGetDepositHistory()
        {
            var context = new NetworkProviderPrivateContext(UserContext.Current);
            var binanceProvider = new BinanceProvider();

            await binanceProvider.GetDepositHistoryAsync(context).ConfigureAwait(false);
        }

        #endregion
    }
}
