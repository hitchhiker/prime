using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Bitfinex;
using Xunit;

namespace Prime.Tests.Providers
{
    public class BitfinexTests : ProviderDirectTestsBase
    {
        public BitfinexTests()
        {
            Provider = Networks.I.Providers.OfType<BitfinexProvider>().FirstProvider();
        }

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
            base.PretestPlaceOrderLimit("XRP_USD".ToAssetPairRaw(), true, 34m, new Money(0.1m, "USD".ToAssetRaw()));
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            base.PretestGetTradeOrderStatus("876236022465");
        }

        [Fact]
        public override void TestGetMarketFromOrder()
        {
            base.PretestGetMarketFromOrder("8083100177");
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: AY: Bitfinex - test with real money.
            base.PretestPlaceWithdrawal(new WalletAddress("6a51d6a5wda6w5d1"), new Money(10000, "UAH".ToAssetRaw()));
        }

        #endregion

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
    }
}
