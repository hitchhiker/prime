using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.NLexch;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class NLexchTests : ProviderDirectTestsBase
    {
        public NLexchTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<NLexchProvider>().FirstProvider();
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
                "doge_btc".ToAssetPairRaw(),
                "uis_btc".ToAssetPairRaw(),
                "ltc_btc".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "doge_btc".ToAssetPairRaw(),
                "uis_btc".ToAssetPairRaw(),
                "ltc_btc".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("uis_btc".ToAssetPairRaw(), true);
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: SC: Not tested with real money
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "doge_btc".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money

            base.PretestPlaceOrderLimit("doge_btc".ToAssetPairRaw(), true, new Money(10, Asset.Btc), new Money(10m, Asset.Btc));
        }
    }
}
