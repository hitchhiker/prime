using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BitMarket;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BitMarketTests : ProviderDirectTestsBase
    {
        public BitMarketTests()
        {
            Provider = Networks.I.Providers.OfType<BitMarketProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var context = new AssetPairs()
            {
                "BTC_PLN".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "LTC_PLN".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                new AssetPair("LiteMineX", "BTC")
            };

            await base.TestGetAssetPairsAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_EUR".ToAssetPairRaw());

            await base.TestGetPrice(context, false).ConfigureAwait(false);
        }
    }
}
