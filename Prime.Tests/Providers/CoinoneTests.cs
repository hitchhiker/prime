using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Coinone;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class CoinoneTests : ProviderDirectTestsBase
    {
        public CoinoneTests()
        {
            Provider = Networks.I.Providers.OfType<CoinoneProvider>().FirstProvider();
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
                "BCH_KRW".ToAssetPairRaw(),
                "QTUM_KRW".ToAssetPairRaw(),
                "LTC_KRW".ToAssetPairRaw(),
                "ETC_KRW".ToAssetPairRaw(),
                "BTC_KRW".ToAssetPairRaw(),
                "ETH_KRW".ToAssetPairRaw(),
                "XRP_KRW".ToAssetPairRaw(),
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
        public override void TestGetPrices()
        {
            var context = new PublicPricesContext(new List<AssetPair>()
            {
                "BCH_KRW".ToAssetPairRaw(),
                "QTUM_KRW".ToAssetPairRaw(),
                "LTC_KRW".ToAssetPairRaw(),
                "ETC_KRW".ToAssetPairRaw(),
                "BTC_KRW".ToAssetPairRaw(),
                "ETH_KRW".ToAssetPairRaw(),
                "XRP_KRW".ToAssetPairRaw(),
            });

            base.TestGetPrices(context);
        }

        [TestMethod]
        public override void TestGetAssetPrices()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "BCH".ToAssetRaw(),
                "QTUM".ToAssetRaw(),
                "LTC".ToAssetRaw(),
                "ETC".ToAssetRaw(),
                "BTC".ToAssetRaw(),
                "ETH".ToAssetRaw(),
                "XRP".ToAssetRaw(),

            }, "KRW".ToAssetRaw());

            base.TestGetAssetPrices(context);
        }
    }
}
