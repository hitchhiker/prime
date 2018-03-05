using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.StocksExchange;
using Xunit;

namespace Prime.Tests.Providers
{
    public class StocksExchangeTests : ProviderDirectTestsBase
    {
        public StocksExchangeTests()
        {
            Provider = Networks.I.Providers.OfType<StocksExchangeProvider>().FirstProvider();
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
                "EAG_BTC".ToAssetPairRaw(),
                "NXT_BTC".ToAssetPairRaw(),
                "STEX_NXT".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "EAG_BTC".ToAssetPairRaw(),
                "NXT_BTC".ToAssetPairRaw(),
                "STEX_NXT".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }
    }
}
