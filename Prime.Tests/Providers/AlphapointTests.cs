using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Alphapoint;
using Xunit;

namespace Prime.Tests.Providers
{
    public class AlphapointTests : ProviderDirectTestsBase
    {
        public AlphapointTests()
        {
            Provider = Networks.I.Providers.OfType<AlphapointProvider>().FirstProvider();
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
                "BTC_USD".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }
    }
}
