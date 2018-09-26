using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Poloniex;
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
            var context = new WalletAddressContext(UserContext.Testing);
            PretestGetAddresses(context);
        }

        [Fact]
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Testing);

            base.PretestGetAddressesForAsset(context);
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // Buy 2 BTC for 0.5 USDT per 1 BTC.
            base.PretestPlaceOrderLimit("USDT_BTC".ToAssetPairRaw(), true, new Money(2, Asset.Btc), new Money(0.5m, Asset.UsdT));

            // Sell 1 XRP for 5 USDT per 1 XRP.
            base.PretestPlaceOrderLimit("USDT_XRP".ToAssetPairRaw(), false, new Money(1, Asset.Xrp), new Money(5, Asset.UsdT));
        }

        [Fact]
        public override void TestGetOrdersHistory()
        {
            base.PretestGetOrdersHistory();
        }

        [Fact]
        public override void TestGetOpenOrders()
        {
            base.PretestGetOpenOrders();
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            base.PretestGetTradeOrderStatus("94628896507");
        }

        [Fact]
        public async void TestCancelOrder()
        {
            var provider = Provider as PoloniexProvider;
            var r = await provider.CancelOrderAsync(new RemoteIdContext(UserContext.Testing, "168325960666"));

            Assert.True(r.Success);
        }

        [Fact]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: AY: test with real money.
            base.PretestPlaceWithdrawal(new WalletAddress("123123"), new Money(1, Asset.UsdT));
        }

        #endregion
    }
}
