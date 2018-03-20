using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Kuna;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class KunaTests : ProviderDirectTestsBase
    {
        public KunaTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<KunaProvider>().FirstProvider();
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
                "btc_uah".ToAssetPairRaw(),
                "eth_uah".ToAssetPairRaw(),
                "kun_btc".ToAssetPairRaw(),
                "waves_uah".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_uah".ToAssetPairRaw(),
                "eth_uah".ToAssetPairRaw(),
                "kun_btc".ToAssetPairRaw(),
                "waves_uah".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("btc_uah".ToAssetPairRaw(), false);
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: SC: Not tested with real money
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "btc_uah".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money
            
            base.PretestPlaceOrderLimit("kun_btc".ToAssetPairRaw(), true, new Money(10, Asset.Btc), new Money(10m, Asset.Btc));
        }
    }
}
