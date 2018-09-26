using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Gate;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class GateTests : ProviderDirectTestsBase
    {
        public GateTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<GateProvider>().FirstProvider();
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
                "etc_eth".ToAssetPairRaw(),
                "pay_btc".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ltc_btc".ToAssetPairRaw(),
                "etc_eth".ToAssetPairRaw(),
                "pay_btc".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetVolume()
        {
            var pairs = new List<AssetPair>()
            {
                "ltc_btc".ToAssetPairRaw(),
                "etc_eth".ToAssetPairRaw(),
                "pay_btc".ToAssetPairRaw()
            };

            base.PretestGetVolume(pairs, false);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("ltc_btc".ToAssetPairRaw(), true);
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "LTC_BTC".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money
            base.PretestPlaceOrderLimit("LTC_BTC".ToAssetPairRaw(), false, new Money(10, Asset.Btc), new Money(1m, Asset.Btc));
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: SC: Not tested with real money
            base.PretestPlaceWithdrawal(new WalletAddress("address_placeholder"), new Money(22, Asset.Btc));
        }
    }
}
