using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.Coinfloor;
using Prime.Tests.Providers;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests
{
    public class CoinfloorTests : ProviderDirectTestsBase
    {
        public CoinfloorTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<CoinfloorProvider>().FirstProvider();
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
                "BTC_USD".ToAssetPairRaw(),
                "BTC_GBP".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_PLN".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
                {
                    "BTC_USD".ToAssetPairRaw(),
                    "BTC_GBP".ToAssetPairRaw(),
                    "BTC_EUR".ToAssetPairRaw(),
                    "BTC_PLN".ToAssetPairRaw()
                };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
        }
    }
}
