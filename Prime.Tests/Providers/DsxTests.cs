using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Dsx;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class DsxTests : ProviderDirectTestsBase
    {
        public DsxTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<DsxProvider>().FirstProvider();
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
                "eth_usd".ToAssetPairRaw(),
                "eth_eur".ToAssetPairRaw(),
                "btc_gbp".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "eth_usd".ToAssetPairRaw(),
                "eth_eur".ToAssetPairRaw(),
                "btc_gbp".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }


        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("eth_usd".ToAssetPairRaw(), false);
        }
    }
}
