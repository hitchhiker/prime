using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Tidex;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class TidexTests : ProviderDirectTestsBase
    {
        public TidexTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<TidexProviderTiLiWe>().FirstProvider();
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
                "eth_btc".ToAssetPairRaw(),
                "dash_btc".ToAssetPairRaw(),
                "doge_btc".ToAssetPairRaw(),
                "bts_btc".ToAssetPairRaw(),
                "waves_btc".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
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
        public override void TestGetTradeOrderStatus()
        {
            base.PretestGetTradeOrderStatus("153525773");
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            base.PretestPlaceOrderLimit("ETH_USDT".ToAssetPairRaw(), true, new Money(1, Asset.Eth), new Money(1, Asset.UsdT));
            base.PretestPlaceOrderLimit("LTC_USDT".ToAssetPairRaw(), true, new Money(1, Asset.Ltc), new Money(1, Asset.UsdT));

            base.PretestPlaceOrderLimit("ETH_USDT".ToAssetPairRaw(), false, new Money(0.01m, Asset.Eth), new Money(3000, Asset.UsdT));
        }

        [Fact]
        public override void TestGetOpenOrders()
        {
            base.PretestGetOpenOrders();

            base.PretestGetOpenOrders("ETH_USDT".ToAssetPairRaw());
        }

        [Fact]
        public override void TestGetOrdersHistory()
        {
            base.PretestGetOrdersHistory();

            base.PretestGetOrdersHistory("ETH_USDT".ToAssetPairRaw());
        }

        [Fact]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        #endregion
    }
}
