using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.NLexch;
using Xunit;

namespace Prime.Tests.Providers
{
    public class NLexchTests : ProviderDirectTestsBase
    {
        public NLexchTests()
        {
            Provider = Networks.I.Providers.OfType<NLexchProvider>().FirstProvider();
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
                "doge_btc".ToAssetPairRaw(),
                "uis_btc".ToAssetPairRaw(),
                "ltc_btc".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "doge_btc".ToAssetPairRaw(),
                "uis_btc".ToAssetPairRaw(),
                "ltc_btc".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("uis_btc".ToAssetPairRaw(), true);
        }
    }
}
