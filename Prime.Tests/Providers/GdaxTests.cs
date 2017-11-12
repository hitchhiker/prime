using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Gdax;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class GdaxTests :ProviderDirectTestsBase
    {
        public GdaxTests()
        {
            Provider = Networks.I.Providers.OfType<GdaxProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            RequiredAssetPairs = new AssetPairs()
            {
                "LTC_EUR".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "LTC_USD".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_GBP".ToAssetPairRaw(),
            };

            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_USD".ToAssetPairRaw());
            await base.TestGetPriceAsync(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetVolumeAsync()
        {
            VolumeContext = new VolumeContext()
            {
                Pair = "LTC_EUR".ToAssetPairRaw()
            };

            await base.TestGetVolumeAsync();
        }
    }
}
