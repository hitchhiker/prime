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
        public override async Task TestPublicApiAsync()
        {
            await base.TestPublicApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
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

            await base.TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_KRW".ToAssetPairRaw());
            await base.TestGetPrice(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "BTC".ToAssetRaw(),
                "ETH".ToAssetRaw(),
                "LTC".ToAssetRaw(),
                "ETC".ToAssetRaw(),
                "XRP".ToAssetRaw()
            }, "KRW".ToAssetRaw());
            await base.TestGetAssetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
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

            await base.TestGetPricesAsync(context).ConfigureAwait(false);
        }
    }
}