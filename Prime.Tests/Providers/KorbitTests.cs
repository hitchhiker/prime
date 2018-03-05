using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Korbit;
using Xunit;

namespace Prime.Tests.Providers
{
    public class KorbitTests : ProviderDirectTestsBase
    {
        public KorbitTests()
        {
            Provider = Networks.I.Providers.OfType<KorbitProvider>().FirstProvider();
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
                "BTC_KRW".ToAssetPairRaw(),
                "ETC_KRW".ToAssetPairRaw(),
                "ETH_KRW".ToAssetPairRaw(),
                "XRP_KRW".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var context = new AssetPairs()
            {
                "BTC_KRW".ToAssetPairRaw(),
                "ETC_KRW".ToAssetPairRaw(),
                "ETH_KRW".ToAssetPairRaw(),
                "XRP_KRW".ToAssetPairRaw(),
            };

            base.PretestGetAssetPairs(context);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_KRW".ToAssetPairRaw(), false);
        }
    }
}
