using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.BxInTh;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BxInThTests : ProviderDirectTestsBase
    {
        public BxInThTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BxInThProvider>().FirstProvider();
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
                "BTC_DOG".ToAssetPairRaw(),
                "BTC_PPC".ToAssetPairRaw(),
                "BTC_XPM".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_DOG".ToAssetPairRaw(),
                "BTC_PPC".ToAssetPairRaw(),
                "BTC_XPM".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_PPC".ToAssetPairRaw(), false);
        }
    }
}
