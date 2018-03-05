using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Gate;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class GateTests : ProviderDirectTestsBase
    {
        public GateTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<GateProvider>().FirstProvider();
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
                "ltc_btc".ToAssetPairRaw(),
                "etc_eth".ToAssetPairRaw(),
                "pay_btc".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ltc_btc".ToAssetPairRaw(),
                "etc_eth".ToAssetPairRaw(),
                "pay_btc".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetVolume()
        {
            var pairs = new List<AssetPair>()
            {
                "ltc_btc".ToAssetPairRaw(),
                "etc_eth".ToAssetPairRaw(),
                "pay_btc".ToAssetPairRaw()
            };

            base.PretestGetVolume(pairs, false);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("ltc_btc".ToAssetPairRaw(), true);
        }
    }
}
