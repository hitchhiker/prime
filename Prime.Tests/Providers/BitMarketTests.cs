using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.BitMarket;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BitMarketTests : ProviderDirectTestsBase
    {
        public BitMarketTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BitMarketProvider>().FirstProvider();
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
                "LTC_PLN".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var context = new AssetPairs()
            {
                "BTC_PLN".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "LTC_PLN".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                new AssetPair("LiteMineX", "BTC")
            };

            base.PretestGetAssetPairs(context);
        }
    }
}
