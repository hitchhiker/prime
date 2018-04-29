using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.BrightonPeak;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BrightonPeakTests : ProviderDirectTestsBase
    {
        public BrightonPeakTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BrightonPeakProvider>().FirstProvider();
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
                "BTC_AUD".ToAssetPairRaw(),
                "LTC_AUD".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_AUD".ToAssetPairRaw(),
                "LTC_AUD".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }


        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_AUD".ToAssetPairRaw(), false);
        }
    }
}
