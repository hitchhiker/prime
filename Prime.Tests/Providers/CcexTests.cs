using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Ccex;
using Xunit;

namespace Prime.Tests.Providers
{
    public class CcexTests : ProviderDirectTestsBase
    {
        public CcexTests()
        {
            Provider = Networks.I.Providers.OfType<CcexProvider>().FirstProvider();
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
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "ZNY_DOGE".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "ZNY_DOGE".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("USD_BTC".ToAssetPairRaw(), true);
        }
    }
}
