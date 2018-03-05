using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Acx;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class AcxTests : ProviderDirectTestsBase
    {
        public AcxTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<AcxProvider>().FirstProvider();
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
                "BCH_AUD".ToAssetPairRaw(),
                "ETH_AUD".ToAssetPairRaw(),
                "HSR_AUD".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_AUD".ToAssetPairRaw(),
                "BCH_AUD".ToAssetPairRaw(),
                "ETH_AUD".ToAssetPairRaw(),
                "HSR_AUD".ToAssetPairRaw()
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
