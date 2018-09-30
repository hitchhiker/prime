using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Ccex;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class CcexTests : ProviderDirectTestsBase
    {
        public CcexTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<CcexProvider>().FirstProvider();
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
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "ZNY_DOGE".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "ZNY_DOGE".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("USD_BTC".ToAssetPairRaw(), true);
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

            //Test for buying
            base.PretestPlaceOrderLimit("GRC_BTC".ToAssetPairRaw(), true, new Money(10, Asset.Btc), new Money(10m, Asset.Btc));

            //Test for selling
            //base.PretestPlaceOrderLimit("GRC_BTC".ToAssetPairRaw(), false, new Money(10, Asset.Btc), new Money(10m, Asset.Btc));
        }
    }
}
