using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Plugins.Services.Btcc;
using Prime.Common;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class BtccTests : ProviderDirectTestsBase
    {
        public BtccTests()
        {
            Provider = Networks.I.Providers.OfType<BtccProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetPrice()
        {
            var context = new PublicPriceContext("BTC_USD".ToAssetPairRaw());
            base.TestGetPrice(context, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
