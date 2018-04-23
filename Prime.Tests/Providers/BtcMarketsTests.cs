using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.BtcMarkets;
using Prime.Tests.Providers;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests
{
    public class BtcMarketsTests : ProviderDirectTestsBase
    {
        public BtcMarketsTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BtcMarketsProvider>().FirstProvider();
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
                "BCH_AUD".ToAssetPairRaw(),
                "BTC_AUD".ToAssetPairRaw(),
                "ETH_AUD".ToAssetPairRaw(),
                "LTC_AUD".ToAssetPairRaw(),
                "XRP_AUD".ToAssetPairRaw(),
                "ETC_AUD".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BCH_AUD".ToAssetPairRaw(),
                "BTC_AUD".ToAssetPairRaw(),
                "ETH_AUD".ToAssetPairRaw(),
                "LTC_AUD".ToAssetPairRaw(),
                "XRP_AUD".ToAssetPairRaw(),
                "ETC_AUD".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }


        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_AUD".ToAssetPairRaw(), false);
        }
    }
}
