using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.ItBit;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class ItBitTests : ProviderDirectTestsBase
    {
        public ItBitTests()
        {
            Provider = Networks.I.Providers.OfType<ItBitProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs(new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BTC_SGD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
            });

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetPrice()
        {
            var context = new PublicPriceContext("BTC_USD".ToAssetPairRaw());
            base.TestGetPrice(context, false);
        }

        [TestMethod]
        public override void TestGetVolume()
        {
            base.TestGetVolume();
        }
    }
}
