using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Yobit;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class YobitTests : ProviderDirectTestsBase
    {
        public YobitTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<YobitProvider>().FirstProvider();
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "ltc_btc".ToAssetPairRaw(),
                "ppc_btc".ToAssetPairRaw(),
                "dash_btc".ToAssetPairRaw(),
                "doge_btc".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ltc_btc".ToAssetPairRaw(),
                "ppc_btc".ToAssetPairRaw(),
                "dash_btc".ToAssetPairRaw(),
                "doge_btc".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("ltc_btc".ToAssetPairRaw(), true);
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: SC: Not tested with real money
            base.PretestPlaceWithdrawal(new WalletAddress("1234"), new Money(22, Asset.Btc));
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: SC: Not tested with real money
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "btc_usd".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money
            base.PretestPlaceOrderLimit("btc_usd".ToAssetPairRaw(), false, new Money(10, Asset.Usd), new Money(10m, Asset.Usd));
        }
    }
}
