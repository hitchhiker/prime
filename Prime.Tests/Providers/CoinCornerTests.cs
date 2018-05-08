using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.CoinCorner;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class CoinCornerTests : ProviderDirectTestsBase
    {
        public CoinCornerTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<CoinCornerProvider>().FirstProvider();
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
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_GBP".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_GBP".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_GBP".ToAssetPairRaw(), false);
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: SC: Not tested with real money
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "BTC_GBP".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money
            base.PretestPlaceOrderLimit("BTC_GBP".ToAssetPairRaw(), true, new Money(10, Assets.I.GetRaw("GBP")), new Money(10m, Assets.I.GetRaw("GBP")));
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: SC: Not tested with real money
            base.PretestPlaceWithdrawal(new WalletAddress("1234"), new Money(22, Assets.I.GetRaw("GBP")));
        }
    }
}
