using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class NovaExchangeTests : ProviderDirectTestsBase
    {/*
        public NovaExchangeTests()
        {
            Provider = Networks.I.Providers.OfType<NovaExchangeProvider>().FirstProvider();
        }

        [Fact]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_ETH".ToAssetPairRaw(),
                "BTC_ETC".ToAssetPairRaw(),
                "LTC_DOGE".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_ETH".ToAssetPairRaw(),
                "BTC_ETC".ToAssetPairRaw(),
                "LTC_DOGE".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }*/
        public NovaExchangeTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
        }
    }
}
