using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.Globitex;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class GlobitexTests : ProviderDirectTestsBase
    {
        public GlobitexTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<GlobitexProvider>().FirstProvider();
        }

        #region Public

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }
       
        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_EUR".ToAssetPairRaw(),
                "BCH_EUR".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_EUR".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }
        
        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_EUR".ToAssetPairRaw(), false);
        }

        #endregion

        #region Private

        [Fact]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }
        
        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: SC: Not tested with real money
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "BTC_EUR".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // TODO: SC: Not tested with real money

            // Test for buying
            base.PretestPlaceOrderLimit("BTC_EUR".ToAssetPairRaw(), true, new Money(10, Asset.Btc), new Money(10m, Asset.Eur));
        }

        [Fact]
        public override void TestGetOrdersHistory()
        {
            // TODO: AY: Sean test with account id as Extra
            
            base.PretestGetOrdersHistory();
        }

        [Fact]
        public override void TestGetOpenOrders()
        {
            // TODO: AY: Sean test with account id as Extra

            base.PretestGetOpenOrders();
        }

        #endregion
    }
}
