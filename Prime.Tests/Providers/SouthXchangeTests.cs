using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.SouthXchange;
using Xunit;

namespace Prime.Tests.Providers
{
    public class SouthXchangeTests : ProviderDirectTestsBase
    {
        public SouthXchangeTests()
        {
            Provider = Networks.I.Providers.OfType<SouthXchangeProvider>().FirstProvider();
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
                "DOGE_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BTG_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "DOGE_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BTG_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("DOGE_BTC".ToAssetPairRaw(), true);
        }
    }
}
