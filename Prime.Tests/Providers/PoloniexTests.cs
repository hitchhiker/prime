using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Poloniex;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class PoloniexTests : ProviderDirectTestsBase
    {
        public PoloniexTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();
        }

        #region Public

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "ETH_ETC".ToAssetPairRaw(),
                "BTC_ETC".ToAssetPairRaw(),
                "USDT_BTC".ToAssetPairRaw(),
            };

            base.PretestGetPricing(pairs, false, false);
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            PretestGetOrderBook("BTC_NXT".ToAssetPairRaw(), false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "ETH_ETC".ToAssetPairRaw(),
                "BTC_ETC".ToAssetPairRaw(),
                "USDT_BTC".ToAssetPairRaw(),
            };

            PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOhlc()
        {
            // BUG: supports only 1 day.
            var context = new OhlcContext(new AssetPair("BTC", "LTC"), TimeResolution.Day,
                new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Hour));

            base.PretestGetOhlc(context);
        }

        #endregion

        #region Private

        [Fact]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [Fact]
        public override void TestGetAddresses()
        {
            var context = new WalletAddressContext(UserContext.Current);
            PretestGetAddresses(context);
        }

        [Fact]
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            base.PretestGetAddressesForAsset(context);
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // base.PretestPlaceOrderLimit("BTC_LTC".ToAssetPairRaw(), true, new Money(1, Asset.Ltc), new Money(1, Asset.Btc));
        }

        [Fact]
        public override void TestGetTradeOrders()
        {
            base.PretestGetTradeOrders();
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            base.PretestGetTradeOrderStatus("123412");
        }

        [Fact]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        #endregion
    }
}
