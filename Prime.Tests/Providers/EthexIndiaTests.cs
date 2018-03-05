using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.EthexIndia;
using Xunit;

namespace Prime.Tests.Providers
{
    public class EthexIndiaTests : ProviderDirectTestsBase
    {
        public EthexIndiaTests()
        {
            Provider = Networks.I.Providers.OfType<EthexIndiaProvider>().FirstProvider();
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
                "ETH_INR".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ETH_INR".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("ETH_INR".ToAssetPairRaw(), false);
        }
    }
}
