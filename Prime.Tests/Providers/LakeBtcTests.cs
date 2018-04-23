using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.LakeBtc;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class LakeBtcTests : ProviderDirectTestsBase
    {
        public LakeBtcTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<LakeBtcProvider>().FirstProvider();
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
                "btc_gbp".ToAssetPairRaw(),
                "btc_cad".ToAssetPairRaw(),
                "btc_chf".ToAssetPairRaw(),
                "btc_ngn".ToAssetPairRaw(),
                "gbp_usd".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_usd".ToAssetPairRaw(),
                "btc_gbp".ToAssetPairRaw(),
                "btc_cad".ToAssetPairRaw(),
                "btc_chf".ToAssetPairRaw(),
                "btc_ngn".ToAssetPairRaw(),
                "gbp_usd".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }
        
        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("btc_usd".ToAssetPairRaw(), false);
        }
    }
}
