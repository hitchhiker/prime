using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Liqui;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class LiquiTests : ProviderDirectTestsBase
    {
        public LiquiTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<LiquiProviderTiLiWe>().FirstProvider();
        }

        #region Public

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "ltc_btc".ToAssetPairRaw(),
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ltc_btc".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw(),
                "ans_btc".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("eth_btc".ToAssetPairRaw(), true);
        }

        #endregion

        #region Private

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

        [Fact]
        public override void TestGetOpenOrders()
        {
            base.PretestGetOpenOrders();
        }

        [Fact]
        public override void TestGetOrdersHistory()
        {
            base.PretestGetOrdersHistory();
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: AY: test using real money - Liqui.
            base.PretestGetTradeOrderStatus("98217034");
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // TODO: AY: test using real money - Liqui.
            base.PretestPlaceOrderLimit("ETH_USDT".ToAssetPairRaw(), true, 1m, new Money(1, Asset.UsdT));
        }

        #endregion
    }
}
