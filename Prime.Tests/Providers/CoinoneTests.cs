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
        public override async Task TestGetAssetPairsAsync()
        {
            RequiredAssetPairs = new AssetPairs()
            {
                "BCH_KRW".ToAssetPairRaw(),
                "QTUM_KRW".ToAssetPairRaw(),
                "LTC_KRW".ToAssetPairRaw(),
                "ETC_KRW".ToAssetPairRaw(),
                "BTC_KRW".ToAssetPairRaw(),
                "ETH_KRW".ToAssetPairRaw(),
                "XRP_KRW".ToAssetPairRaw(),
            };

            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_KRW".ToAssetPairRaw());
            await base.TestGetPriceAsync(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
        {
            PublicPricesContext = new PublicPricesContext(new List<AssetPair>()
            {
                "BCH_KRW".ToAssetPairRaw(),
                "QTUM_KRW".ToAssetPairRaw(),
                "LTC_KRW".ToAssetPairRaw(),
                "ETC_KRW".ToAssetPairRaw(),
                "BTC_KRW".ToAssetPairRaw(),
                "ETH_KRW".ToAssetPairRaw(),
                "XRP_KRW".ToAssetPairRaw(),
            });

            await base.TestGetPricesAsync();
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            PublicAssetPricesContext = new PublicAssetPricesContext(new List<Asset>()
            {
                "BCH".ToAssetRaw(),
                "QTUM".ToAssetRaw(),
                "LTC".ToAssetRaw(),
                "ETC".ToAssetRaw(),
                "BTC".ToAssetRaw(),
                "ETH".ToAssetRaw(),
                "XRP".ToAssetRaw(),

            }, "KRW".ToAssetRaw());

            await base.TestGetAssetPricesAsync();
        }
    }
}
