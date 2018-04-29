using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.IndependentReserve;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class IndependentReserveTests : ProviderDirectTestsBase
    {
        public IndependentReserveTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<IndependentReserveProvider>().FirstProvider();
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
                "btc_aud".ToAssetPairRaw(),
                "btc_nzd".ToAssetPairRaw(),
                "eth_usd".ToAssetPairRaw(),
                "eth_aud".ToAssetPairRaw(),
                "eth_nzd".ToAssetPairRaw(),
                "bch_usd".ToAssetPairRaw(),
                "bch_aud".ToAssetPairRaw(),
                "bch_nzd".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_usd".ToAssetPairRaw(),
                "btc_aud".ToAssetPairRaw(),
                "btc_nzd".ToAssetPairRaw(),
                "eth_usd".ToAssetPairRaw(),
                "eth_aud".ToAssetPairRaw(),
                "eth_nzd".ToAssetPairRaw(),
                "bch_usd".ToAssetPairRaw(),
                "bch_aud".ToAssetPairRaw(),
                "bch_nzd".ToAssetPairRaw()
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
