using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Luno;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class LunoTests : ProviderDirectTestsBase
    {
        public LunoTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<LunoProvider>().FirstProvider();
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
                "XBT_MYR".ToAssetPairRaw(),
                "XBT_IDR".ToAssetPairRaw(),
                "XBT_NGN".ToAssetPairRaw(),
                "XBT_ZAR".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "XBT_MYR".ToAssetPairRaw(),
                "XBT_IDR".ToAssetPairRaw(),
                "XBT_NGN".ToAssetPairRaw(),
                "XBT_ZAR".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("XBT_MYR".ToAssetPairRaw(), false);
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: SC: Not tested with real money
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "XBT_ZAR".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money
            base.PretestPlaceOrderLimit("ETH_XBT".ToAssetPairRaw(), true, new Money(10, Assets.I.GetRaw("XBT")), new Money(10m, Assets.I.GetRaw("XBT")));
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: SC: Not tested with real money
            base.PretestPlaceWithdrawal(new WalletAddress("1234"), new Money(22, Asset.Eth));
        }
    }
}
