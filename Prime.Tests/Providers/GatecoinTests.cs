using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Gatecoin;
using Xunit;

namespace Prime.Tests.Providers
{
    public class GatecoinTests : ProviderDirectTestsBase
    {
        public GatecoinTests()
        {
            Provider = Networks.I.Providers.OfType<GatecoinProvider>().FirstProvider();
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
                "BTC_USD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
        }
    }
}
