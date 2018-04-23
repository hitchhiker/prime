using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.TheRockTrading;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class TheRockTradingTests : ProviderDirectTestsBase
    {
        public TheRockTradingTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<TheRockTradingProvider>().FirstProvider();
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
                "LTC_EUR".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "BTC_XRP".ToAssetPairRaw(),
                "EUR_XRP".ToAssetPairRaw(),
                "USD_XRP".ToAssetPairRaw(),
                "PPC_EUR".ToAssetPairRaw(),
                "PPC_BTC".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "ZEC_BTC".ToAssetPairRaw(),
                "ZEC_EUR".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "LTC_EUR".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "BTC_XRP".ToAssetPairRaw(),
                "EUR_XRP".ToAssetPairRaw(),
                "USD_XRP".ToAssetPairRaw(),
                "PPC_EUR".ToAssetPairRaw(),
                "PPC_BTC".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "ZEC_BTC".ToAssetPairRaw(),
                "ZEC_EUR".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_EUR".ToAssetPairRaw(), false);
        }
    }
}
