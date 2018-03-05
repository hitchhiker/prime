using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.MixCoins;
using Xunit;

namespace Prime.Tests.Providers
{
    public class MixCoinsTests : ProviderDirectTestsBase
    {
        public MixCoinsTests()
        {
            Provider = Networks.I.Providers.OfType<MixCoinsProvider>().FirstProvider();
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
                "btc_usd".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw(),
                "bcc_btc".ToAssetPairRaw(),
                "lsk_btc".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_usd".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw(),
                "bcc_btc".ToAssetPairRaw(),
                "lsk_btc".ToAssetPairRaw()
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
