using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.Exx;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class ExxTests : ProviderDirectTestsBase
    {
        public ExxTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<ExxProvider>().FirstProvider();
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
                "bts_eth".ToAssetPairRaw(),
                "BTM_ETH".ToAssetPairRaw(),
                "EOS_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "bts_eth".ToAssetPairRaw(),
                "BTM_ETH".ToAssetPairRaw(),
                "EOS_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("EOS_BTC".ToAssetPairRaw(), true);
        }
    }
}
