using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Paymium;
using Xunit;

namespace Prime.Tests.Providers
{
    public class PaymiumTests : ProviderDirectTestsBase
    {
        public PaymiumTests()
        {
            Provider = Networks.I.Providers.OfType<PaymiumProvider>().FirstProvider();
        }

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
                "EUR_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_EUR".ToAssetPairRaw(),
                "EUR_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_EUR".ToAssetPairRaw(), false);
        }
    }
}
