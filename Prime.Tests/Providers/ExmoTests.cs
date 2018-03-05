using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Exmo;
using Xunit;

namespace Prime.Tests.Providers
{
    public class ExmoTests : ProviderDirectTestsBase
    {
        public ExmoTests()
        {
            Provider = Networks.I.Providers.OfType<ExmoProvider>().FirstProvider();
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
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "BTC_RUB".ToAssetPairRaw(),
                "XRP_USD".ToAssetPairRaw(),
                "BTC_USDT".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "BTC_RUB".ToAssetPairRaw(),
                "XRP_USD".ToAssetPairRaw(),
                "BTC_USDT".ToAssetPairRaw()
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
