using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Bitso;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class BitsoTests : ProviderDirectTestsBase
    {
        public BitsoTests()
        {
            Provider = Networks.I.Providers.OfType<BitsoProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("eth_btc".ToAssetPairRaw());
            await base.TestGetPriceAsync(context, true).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var requiredPairs = new AssetPairs()
            {
                "eth_mxn".ToAssetPairRaw(),
                "xrp_btc".ToAssetPairRaw(),
                "xrp_mxn".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw(),
                "bch_btc".ToAssetPairRaw()
            };

            await base.TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }
    }
}
