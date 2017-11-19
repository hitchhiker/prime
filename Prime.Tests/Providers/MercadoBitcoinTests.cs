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
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetPrice()
        {
            var context = new PublicPriceContext("BTC_BLR".ToAssetPairRaw());
            base.TestGetPrice(context, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_BLR".ToAssetPairRaw(),
                "LTC_BLR".ToAssetPairRaw(),
                "BCH_BLR".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
