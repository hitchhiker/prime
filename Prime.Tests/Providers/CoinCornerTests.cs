using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.CoinCorner;
using Xunit;

namespace Prime.Tests.Providers
{
    public class CoinCornerTests : ProviderDirectTestsBase
    {
        public CoinCornerTests()
        {
            Provider = Networks.I.Providers.OfType<CoinCornerProvider>().FirstProvider();
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
                "BTC_GBP".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_GBP".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_GBP".ToAssetPairRaw(), false);
        }
    }
}
