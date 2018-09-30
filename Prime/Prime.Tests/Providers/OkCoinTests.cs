using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.OkCoin;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class OkCoinTests : ProviderDirectTestsBase
    {
        public OkCoinTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<OkCoinProvider>().FirstProvider();
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
                "btc_usd".ToAssetPairRaw(),
                "ltc_usd".ToAssetPairRaw(),
                "eth_usd".ToAssetPairRaw(),
                "etc_usd".ToAssetPairRaw(),
                "bch_usd".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_usd".ToAssetPairRaw(),
                "ltc_usd".ToAssetPairRaw(),
                "eth_usd".ToAssetPairRaw(),
                "etc_usd".ToAssetPairRaw(),
                "bch_usd".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("btc_usd".ToAssetPairRaw(), false);
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

            base.PretestPlaceOrderLimit("btc_usd".ToAssetPairRaw(), true, new Money(10, Asset.Usd), new Money(10m, Asset.Usd));
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: SC: Not tested with real money
            base.PretestPlaceWithdrawal(new WalletAddress("1234"), new Money(22, Asset.Btc));
        }
    }
}
