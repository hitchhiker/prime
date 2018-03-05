using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Bitsane;
using Xunit;

namespace Prime.Tests.Providers
{
    public class BitsaneTests : ProviderDirectTestsBase
    {
        public BitsaneTests()
        {
            Provider = Networks.I.Providers.OfType<BitsaneProvider>().FirstProvider();
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
                "BTC_USD".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
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
