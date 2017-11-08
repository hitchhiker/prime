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
            RequiredAssetPairs = new AssetPairs()
            {
                "BTC_PLN".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "LTC_PLN".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                new AssetPair("LiteMineX", "BTC")
            };
            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            PublicPriceContext = new PublicPriceContext("LiteMineX_BTC".ToAssetPairRaw());

            await base.TestGetPriceAsync();
        }
    }
}
