using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.MercadoBitcoin;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class MercadoBitcoinTests : ProviderDirectTestsBase
    {
        public MercadoBitcoinTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<MercadoBitcoinProvider>().FirstProvider();
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
                "BTC_BLR".ToAssetPairRaw(),
                "LTC_BLR".ToAssetPairRaw(),
                "BCH_BLR".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_BLR".ToAssetPairRaw(),
                "LTC_BLR".ToAssetPairRaw(),
                "BCH_BLR".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_BLR".ToAssetPairRaw(), false);
        }
    }
}
