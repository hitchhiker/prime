using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Tidex;
using Xunit;

namespace Prime.Tests.Providers
{
    public class TidexTests : ProviderDirectTestsBase
    {
        public TidexTests()
        {
            Provider = Networks.I.Providers.OfType<TidexProviderTiLiWe>().FirstProvider();
        }

        #region Private

        [Fact]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            base.PretestGetTradeOrderStatus("98217034");
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            base.PretestPlaceOrderLimit("ETH_USDT".ToAssetPairRaw(), true, 1m, new Money(1, Asset.UsdT));
        }

        [Fact]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
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
    }
}
