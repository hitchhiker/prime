using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BitStamp;
using Prime.Plugins.Services.QuadrigaCX;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class QuadrigaCxTests : ProviderDirectTestsBase
    {
        public QuadrigaCxTests()
        {
            Provider = Networks.I.Providers.OfType<QuadrigaCxProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetPrice()
        {
            var context = new PublicPriceContext("btc_usd".ToAssetPairRaw());
            base.TestGetPrice(context, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BTC_CAD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "ETH_CAD".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
