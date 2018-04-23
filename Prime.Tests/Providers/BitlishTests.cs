using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.Bitlish;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BitlishTests : ProviderDirectTestsBase
    {
        public BitlishTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BitlishProvider>().FirstProvider();
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
                "btc_usd".ToAssetPairRaw(),
                "dsh_eur".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_eur".ToAssetPairRaw(),
                "btc_usd".ToAssetPairRaw(),
                "dsh_eur".ToAssetPairRaw()
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
