using System.Collections.Generic;
using System.Linq;
using Prime.Core;
using Prime.Finance.Services.Services.MixCoins;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class MixCoinsTests : ProviderDirectTestsBase
    {
        public MixCoinsTests(ITestOutputHelper outputWriter) : base(outputWriter)
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
