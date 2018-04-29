using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Bisq;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BisqTests : ProviderDirectTestsBase
    {
        public BisqTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BisqProvider>().FirstProvider();
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
                "btc_eur".ToAssetPairRaw(),
                "btc_nok".ToAssetPairRaw(),
                "btc_aud".ToAssetPairRaw(),
                "etc_btc".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_eur".ToAssetPairRaw(),
                "btc_nok".ToAssetPairRaw(),
                "btc_aud".ToAssetPairRaw(),
                "etc_btc".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("btc_eur".ToAssetPairRaw(), false);
        }
    }
}
