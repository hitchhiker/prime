using System.Collections.Generic;
using System.Linq;
using Prime.Core;
using Prime.Finance.Services.Services.Acx;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class AcxTests : ProviderDirectTestsBase
    {
        public AcxTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<AcxProvider>().FirstProvider();
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
                "BTC_AUD".ToAssetPairRaw(),
                "BCH_AUD".ToAssetPairRaw(),
                "ETH_AUD".ToAssetPairRaw(),
                "HSR_AUD".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_AUD".ToAssetPairRaw(),
                "BCH_AUD".ToAssetPairRaw(),
                "ETH_AUD".ToAssetPairRaw(),
                "HSR_AUD".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_AUD".ToAssetPairRaw(), false);
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: SC: Not tested with real money
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "BTC_AUD".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money
            base.PretestPlaceOrderLimit("BTC_AUD".ToAssetPairRaw(), true, new Money(10, Assets.I.GetRaw("AUD")), new Money(10m, Assets.I.GetRaw("AUD")));
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: SC: Not tested with real money
            base.PretestPlaceWithdrawal(new WalletAddress("1234"), new Money(22, Asset.Btc));
        }
    }
}
