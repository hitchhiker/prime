using System.Collections.Generic;
using System.Linq;
using Prime.Core;
using Prime.Finance.Services.Services.Kucoin;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class KucoinTests : ProviderDirectTestsBase
    {
        public KucoinTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<KucoinProvider>().FirstProvider();
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
                "KCS_BTC".ToAssetPairRaw(),
                "KNC_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "KCS_BTC".ToAssetPairRaw(),
                "KNC_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("KCS_BTC".ToAssetPairRaw(), true);
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: SC: Not tested with real money
            base.PretestPlaceWithdrawal(new WalletAddress("address_placeholder"), new Money(22, Asset.Usd));
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "KCS_BTC".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money
            base.PretestPlaceOrderLimit("KCS_BTC".ToAssetPairRaw(), false, 1, new Money(1m, Asset.Btc));
        }
    }
}
