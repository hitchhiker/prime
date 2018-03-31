using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Bitso;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BitsoTests : ProviderDirectTestsBase
    {
        public BitsoTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BitsoProvider>().FirstProvider();
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
                "eth_btc".ToAssetPairRaw(),
                "eth_mxn".ToAssetPairRaw(),
                "xrp_btc".ToAssetPairRaw(),
                "xrp_mxn".ToAssetPairRaw(),
                "bch_btc".ToAssetPairRaw()
            };
            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "eth_mxn".ToAssetPairRaw(),
                "xrp_btc".ToAssetPairRaw(),
                "xrp_mxn".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw(),
                "bch_btc".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("eth_btc".ToAssetPairRaw(), true);
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
            base.PretestGetTradeOrderStatus(orderId, "eth_btc".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money
            base.PretestPlaceOrderLimit("eth_btc".ToAssetPairRaw(), true, new Money(10, Asset.Btc), new Money(10m, Asset.Btc));
        }
    }
}
