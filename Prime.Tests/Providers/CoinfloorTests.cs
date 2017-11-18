using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Coinfloor;
using Prime.Tests.Providers;

namespace Prime.Tests
{
    [TestClass()]
    public class CoinfloorTests : ProviderDirectTestsBase
    {
        public CoinfloorTests()
        {
            Provider = Networks.I.Providers.OfType<CoinfloorProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestPublicApiAsync()
        {
            await base.TestPublicApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("xbt_usd".ToAssetPairRaw());
            await base.TestGetPrice(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var requiredPairs = new AssetPairs()
                {
                    "XBT_USD".ToAssetPairRaw(),
                    "XBT_GBP".ToAssetPairRaw(),
                    "XBT_EUR".ToAssetPairRaw(),
                    "XBT_PLN".ToAssetPairRaw()
                };

            await base.TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }
    }
}
