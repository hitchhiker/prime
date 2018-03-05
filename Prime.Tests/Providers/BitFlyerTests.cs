using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.BitFlyer;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BitFlyerTests : ProviderDirectTestsBase
    {
        public BitFlyerTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BitFlyerProvider>().FirstProvider();
        }

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var context = new AssetPairs()
            {
                "BTC_JPY".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(context);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_JPY".ToAssetPairRaw(), false);
        }
    }
}
