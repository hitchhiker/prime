using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Cex;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class CexTests : ProviderDirectTestsBase
    {
        public CexTests()
        {
            Provider = Networks.I.Providers.OfType<CexProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestPublicApiAsync()
        {
            await base.TestPublicApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var context = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_RUB".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            await base.TestGetAssetPairsAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_USD".ToAssetPairRaw());
            await base.TestGetPriceAsync(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
        {
            var context = new PublicPricesContext(new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_RUB".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            });
            await base.TestGetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "GHS".ToAssetRaw(),
                "ZEC".ToAssetRaw(),
                "DASH".ToAssetRaw(),
                "BCH".ToAssetRaw()
            }, Asset.Btc);

            await base.TestGetAssetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetVolumeAsync()
        {
            var context = new VolumeContext()
            {
                Pair = "BTC_USD".ToAssetPairRaw()
            };
            await base.TestGetVolumeAsync(context).ConfigureAwait(false);
        }
    }
}
