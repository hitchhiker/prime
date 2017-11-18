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
        public override async Task TestPublicApiAsync()
        {
            await base.TestPublicApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
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

            await base.TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_KRW".ToAssetPairRaw());
            await base.TestGetPrice(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
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

            await base.TestGetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
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

            await base.TestGetAssetPricesAsync(context).ConfigureAwait(false);
        }
    }
}
