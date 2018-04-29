using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Abucoins;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class AbucoinsTests : ProviderDirectTestsBase
    {
        public AbucoinsTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<AbucoinsProvider>().FirstProvider();
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
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetVolume()
        {
            var pairs = new List<AssetPair>()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
            };

            base.PretestGetVolume(pairs, false);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_PLN".ToAssetPairRaw(), false);

            Assert.Throws<ApiResponseException>(() =>
            {
                base.PretestGetOrderBook("BTC_PLNaw".ToAssetPairRaw(), false);
            });
        }
    }
}
