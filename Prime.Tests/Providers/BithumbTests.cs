using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Bithumb;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BithumbTests : ProviderDirectTestsBase
    {
        public BithumbTests()
        {
            Provider = Networks.I.Providers.OfType<BithumbProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_KRW".ToAssetPairRaw(),
                "ETH_KRW".ToAssetPairRaw(),
                "LTC_KRW".ToAssetPairRaw(),
                "ETC_KRW".ToAssetPairRaw(),
                "XRP_KRW".ToAssetPairRaw(),
                "QTUM_KRW".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_KRW".ToAssetPairRaw(),
                "ETH_KRW".ToAssetPairRaw(),
                "LTC_KRW".ToAssetPairRaw(),
                "ETC_KRW".ToAssetPairRaw(),
                "XRP_KRW".ToAssetPairRaw(),
                "QTUM_KRW".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetPrice()
        {
            var context = new PublicPriceContext("BTC_KRW".ToAssetPairRaw());
            base.TestGetPrice(context, false);
        }

        [TestMethod]
        public override void TestGetAssetPrices()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "BTC".ToAssetRaw(),
                "ETH".ToAssetRaw(),
                "LTC".ToAssetRaw(),
                "ETC".ToAssetRaw(),
                "XRP".ToAssetRaw()
            }, "KRW".ToAssetRaw());
            base.TestGetAssetPrices(context);
        }

        [TestMethod]
        public override void TestGetPrices()
        {
            var context = new PublicPricesContext(new List<AssetPair>()
            {
                "BTC_KRW".ToAssetPairRaw(),
                "ETH_KRW".ToAssetPairRaw(),
                "LTC_KRW".ToAssetPairRaw(),
                "ETC_KRW".ToAssetPairRaw(),
                "XRP_KRW".ToAssetPairRaw(),
                "QTUM_KRW".ToAssetPairRaw()
            });

            base.TestGetPrices(context);
        }
    }
}