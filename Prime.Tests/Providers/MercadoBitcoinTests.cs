using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.MercadoBitcoin;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class MercadoBitcoinTests : ProviderDirectTestsBase
    {
        public MercadoBitcoinTests()
        {
            Provider = Networks.I.Providers.OfType<MercadoBitcoinProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestPublicApiAsync()
        {
            await base.TestPublicApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_BLR".ToAssetPairRaw());
            await base.TestGetPrice(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_BLR".ToAssetPairRaw(),
                "LTC_BLR".ToAssetPairRaw(),
                "BCH_BLR".ToAssetPairRaw()
            };

            await base.TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }
    }
}
