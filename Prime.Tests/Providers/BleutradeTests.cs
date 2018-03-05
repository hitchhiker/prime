using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Bleutrade;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BleutradeTests : ProviderDirectTestsBase
    {
        public BleutradeTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BleutradeProvider>().FirstProvider();
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
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ADC_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var context = new AssetPairs()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ADC_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(context);
        }

        [Fact]
        public override void TestGetVolume()
        {
            var pairs = new List<AssetPair>()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ADC_BTC".ToAssetPairRaw()
            };

            base.PretestGetVolume(pairs, false);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("LTC_BTC".ToAssetPairRaw(), true);
        }
    }
}
