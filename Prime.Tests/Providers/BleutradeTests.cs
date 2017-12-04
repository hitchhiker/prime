using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Bleutrade;
using Prime.Plugins.Services.Cex;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BleutradeTests : ProviderDirectTestsBase
    {
        public BleutradeTests()
        {
            Provider = Networks.I.Providers.OfType<BleutradeProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ADC_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var context = new AssetPairs()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ADC_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(context);
        }

        [TestMethod]
        public override void TestGetVolume()
        {
            var pairs = new List<AssetPair>()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ADC_BTC".ToAssetPairRaw()
            };

            base.TestGetVolume(pairs, false);
        }
    }
}
