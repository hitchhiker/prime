using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Coincheck;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class CoincheckTests : ProviderDirectTestsBase
    {
        public CoincheckTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<CoincheckProvider>().FirstProvider();
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
                "btc_jpy".ToAssetPairRaw(),
                "eth_jpy".ToAssetPairRaw(),
                "etc_jpy".ToAssetPairRaw(),
                "dao_jpy".ToAssetPairRaw(),
                "lsk_jpy".ToAssetPairRaw(),
                "fct_jpy".ToAssetPairRaw(),
                "xmr_jpy".ToAssetPairRaw(),
                "rep_jpy".ToAssetPairRaw(),
                "xrp_jpy".ToAssetPairRaw(),
                "zec_jpy".ToAssetPairRaw(),
                "xem_jpy".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
        {
            "btc_jpy".ToAssetPairRaw(),
            "eth_jpy".ToAssetPairRaw(),
            "etc_jpy".ToAssetPairRaw(),
            "dao_jpy".ToAssetPairRaw(),
            "lsk_jpy".ToAssetPairRaw(),
            "fct_jpy".ToAssetPairRaw(),
            "xmr_jpy".ToAssetPairRaw(),
            "rep_jpy".ToAssetPairRaw(),
            "xrp_jpy".ToAssetPairRaw(),
            "zec_jpy".ToAssetPairRaw(),
            "xem_jpy".ToAssetPairRaw()
        };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("btc_jpy".ToAssetPairRaw(), false);
        }
    }
}
