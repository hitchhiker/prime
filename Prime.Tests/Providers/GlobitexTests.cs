using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Globitex;
using Xunit;

namespace Prime.Tests.Providers
{
    public class GlobitexTests : ProviderDirectTestsBase
    {
        public GlobitexTests()
        {
            Provider = Networks.I.Providers.OfType<GlobitexProvider>().FirstProvider();
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
                "BTC_EUR".ToAssetPairRaw()
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
    }
}
