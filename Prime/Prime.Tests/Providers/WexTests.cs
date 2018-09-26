using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Wex;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class WexTests : ProviderDirectTestsBase
    {
        public WexTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<WexProvider>().FirstProvider();
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
                "btc_eur".ToAssetPairRaw(),
                "btc_usd".ToAssetPairRaw(),
                "ltc_usd".ToAssetPairRaw(),
                "ltc_eur".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_eur".ToAssetPairRaw(),
                "btc_usd".ToAssetPairRaw(),
                "ltc_usd".ToAssetPairRaw(),
                "ltc_eur".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("btc_usd".ToAssetPairRaw(), false);
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
        public override void TestGetTradeOrderStatus()
        {
            base.PretestGetTradeOrderStatus("98217034");
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // TODO: AY: Wex - test with real money.
            base.PretestPlaceOrderLimit("BTC_USD".ToAssetPairRaw(), true, new Money(1, Asset.Btc), new Money(1, Asset.Usd));

            base.PretestPlaceOrderLimit("BTC_USD".ToAssetPairRaw(), false, new Money(0.0001m, Asset.Btc), new Money(100000, Asset.Usd));
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
        public override void TestPlaceWithdrawal()
        {
            // TODO: AY: Wex - test with real money.
            base.PretestPlaceWithdrawal(new WalletAddress("testaddress"), new Money(1, Asset.Usd));
        }

        #endregion
    }
}
