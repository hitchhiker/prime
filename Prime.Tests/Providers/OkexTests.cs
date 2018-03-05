using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Okex;
using Xunit;

namespace Prime.Tests.Providers
{
    public class OkexTests : ProviderDirectTestsBase
    {
        public OkexTests()
        {
            Provider = Networks.I.Providers.OfType<OkexProvider>().FirstProvider();
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
                "eth_btc".ToAssetPairRaw(),
                "etc_btc".ToAssetPairRaw(),
                "bch_btc".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ltc_btc".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw(),
                "etc_btc".ToAssetPairRaw(),
                "bch_btc".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }
    }
}
