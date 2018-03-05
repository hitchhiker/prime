using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Kuna;
using Xunit;

namespace Prime.Tests.Providers
{
    public class KunaTests : ProviderDirectTestsBase
    {
        public KunaTests()
        {
            Provider = Networks.I.Providers.OfType<KunaProvider>().FirstProvider();
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
                "btc_uah".ToAssetPairRaw(),
                "eth_uah".ToAssetPairRaw(),
                "kun_btc".ToAssetPairRaw(),
                "waves_uah".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_uah".ToAssetPairRaw(),
                "eth_uah".ToAssetPairRaw(),
                "kun_btc".ToAssetPairRaw(),
                "waves_uah".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("btc_uah".ToAssetPairRaw(), false);
        }
    }
}
