using System.Collections.Generic;
using System.Linq;
using Prime.Core;
using Prime.Finance.Services.Services.Bitfinex;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BitfinexTests : ProviderDirectTestsBase
    {
        public BitfinexTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BitfinexProvider>().FirstProvider();
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
                "BTC_USD".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
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
        public override void TestPlaceOrderLimit()
        {
            // Reliable sell order test.
            base.PretestPlaceOrderLimit("BTC_USD".ToAssetPairRaw(), false, new Money(0.002m, Asset.Btc), new Money(100_000m, Asset.Usd));

            // Reliable buy order test.
            base.PretestPlaceOrderLimit("XRP_BTC".ToAssetPairRaw(), true, new Money(22, Asset.Xrp), new Money(0.000001m, Asset.Btc));
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            base.PretestGetTradeOrderStatus("9991645259");
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: AY: Bitfinex - test with real money.
            base.PretestPlaceWithdrawal(new WalletAddress("6a51d6a5wda6w5d1"), new Money(10000, "UAH".ToAssetRaw()));
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

        #endregion
    }
}
