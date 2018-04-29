using System.Collections.Generic;
using System.Linq;
using Prime.Core;
using Prime.Finance.Services.Services.Luno;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class LunoTests : ProviderDirectTestsBase
    {
        public LunoTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<LunoProvider>().FirstProvider();
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
                "XBT_MYR".ToAssetPairRaw(),
                "XBT_IDR".ToAssetPairRaw(),
                "XBT_NGN".ToAssetPairRaw(),
                "XBT_ZAR".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "XBT_MYR".ToAssetPairRaw(),
                "XBT_IDR".ToAssetPairRaw(),
                "XBT_NGN".ToAssetPairRaw(),
                "XBT_ZAR".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("XBT_MYR".ToAssetPairRaw(), false);
        }
    }
}
