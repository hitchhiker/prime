using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.BlinkTrade;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BlinkTradeTests : ProviderDirectTestsBase
    {
        public BlinkTradeTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BlinkTradeProvider>().FirstProvider();
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
                "BTC_VND".ToAssetPairRaw(),
                "BTC_BRL".ToAssetPairRaw(),
                "BTC_PKR".ToAssetPairRaw(),
                "BTC_CLP".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_VEF".ToAssetPairRaw(),
                "BTC_VND".ToAssetPairRaw(),
                "BTC_BRL".ToAssetPairRaw(),
                "BTC_PKR".ToAssetPairRaw(),
                "BTC_CLP".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_VEF".ToAssetPairRaw(), false);
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: SC: Not tested with real money
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "BTC_VEF".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money

            base.PretestPlaceOrderLimit("BTC_VEF".ToAssetPairRaw(), true, new Money(10, Assets.I.GetRaw("VEF")), new Money(10m, Assets.I.GetRaw("VEF")));
        }
    }
}
