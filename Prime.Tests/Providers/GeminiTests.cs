using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Gemini;
using Xunit;

namespace Prime.Tests.Providers
{
    public class GeminiTests : ProviderDirectTestsBase
    {
        public GeminiTests()
        {
            Provider = Networks.I.Providers.OfType<GeminiProvider>().FirstProvider();
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
                "BTC_USD".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }
    }
}
