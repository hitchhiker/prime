using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Coinsecure;
using Xunit;

namespace Prime.Tests.Providers
{
    public class CoinsecureTests : ProviderDirectTestsBase
    {
        public CoinsecureTests()
        {
            Provider = Networks.I.Providers.OfType<CoinsecureProvider>().FirstProvider();
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
                "BTC_INR".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_INR".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }
    }
}
