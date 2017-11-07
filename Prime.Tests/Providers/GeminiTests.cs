using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Gemini;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class GeminiTests : ProviderDirectTestsBase
    {
        public GeminiTests()
        {
            Provider = Networks.I.Providers.OfType<GeminiProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            RequiredAssetPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            PublicPriceContext = new PublicPriceContext("BTC_USD".ToAssetPairRaw());

            await base.TestGetPriceAsync();
        }
    }
}
