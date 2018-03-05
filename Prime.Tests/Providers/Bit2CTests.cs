using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Bit2C;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class Bit2CTests : ProviderDirectTestsBase
    {
        public Bit2CTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<Bit2CProvider>().FirstProvider();
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
                "BTC_NIS".ToAssetPairRaw(),
                "LTC_NIS".ToAssetPairRaw(),
                "BCH_NIS".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_NIS".ToAssetPairRaw(),
                "LTC_NIS".ToAssetPairRaw(),
                "BCH_NIS".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }


        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_NIS".ToAssetPairRaw(), false);
        }
    }
}
