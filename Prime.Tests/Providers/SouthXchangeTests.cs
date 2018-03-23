using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.SouthXchange;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class SouthXchangeTests : ProviderDirectTestsBase
    {
        public SouthXchangeTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<SouthXchangeProvider>().FirstProvider();
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
                "DOGE_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BTG_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "DOGE_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BTG_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("DOGE_BTC".ToAssetPairRaw(), true);
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money
            base.PretestPlaceOrderLimit("DOGE_BTC".ToAssetPairRaw(), false, new Money(10m, Asset.Btc), new Money(10m, Asset.Btc));
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: SC: Not tested with real money
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "DOGE_BTC".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: SC: Not tested with real money
            base.PretestPlaceWithdrawal(new WalletAddress("1234"), new Money(22, Asset.Btc));
        }
    }
}
