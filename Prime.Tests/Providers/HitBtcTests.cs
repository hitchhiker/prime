using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.HitBtc;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class HitBtcTests : ProviderDirectTestsBase
    {
        public HitBtcTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<HitBtcProvider>().FirstProvider();
        }

        #region Public

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(), // On exchange it is BTC/USDT.
                "DOGE_BTC".ToAssetPairRaw(),
                "DASH_ETH".ToAssetPairRaw(),
                "DASH_USD".ToAssetPairRaw(), // On exchange it is DASH/USDT.
                "ETH_USD".ToAssetPairRaw() // On exchange it is ETH/USDT.
            };

            base.PretestGetPricing(pairs, false, false);
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(), // On exchange it is BTC/USDT.
                "DOGE_BTC".ToAssetPairRaw(),
                "DASH_ETH".ToAssetPairRaw(),
                "DASH_USD".ToAssetPairRaw(), // On exchange it is DASH/USDT.
                "ETH_USD".ToAssetPairRaw() // On exchange it is ETH/USDT.
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        #endregion

        #region Private

        [Fact]
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext("BTC".ToAssetRaw(), UserContext.Current);
            base.PretestGetAddressesForAsset(context);
        }

        [Fact]
        public override void TestGetOrdersHistory()
        {
            // BUG: AY: exchange returns ETH_USDT orders when setting ETH_USD market. ETH_USDT is not recognized by exchange.
            base.PretestGetOrdersHistory("ETH_USD".ToAssetPairRaw());

            // However, this market works.
            base.PretestGetOrdersHistory("XRP_USDT".ToAssetPairRaw());
        }

        [Fact]
        public override void TestGetOpenOrders()
        {
            base.PretestGetOpenOrders("XRP_USDT".ToAssetPairRaw());
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            base.PretestGetTradeOrderStatus("aa6074891d8b12d27bf0162bae189ff0");
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            base.PretestPlaceOrderLimit("XRP_USDT".ToAssetPairRaw(), false, new Money(1, Asset.Xrp), new Money(1000m, Asset.UsdT));

            base.PretestPlaceOrderLimit("XRP_USDT".ToAssetPairRaw(), true, new Money(1, Asset.Xrp), new Money(0.001m, Asset.UsdT));
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: AY: test using real money - HitBtc.
            base.PretestPlaceWithdrawal(new WalletAddress("123"), new Money(1, Asset.Btc));
        }

        [Fact]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [Fact]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        #endregion
    }
}
