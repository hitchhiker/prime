using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.ShapeShift;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class ShapeShiftTests : ProviderDirectTestsBase
    {
        public ShapeShiftTests()
        {
            Provider = Networks.I.Providers.OfType<ShapeShiftProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_ETH".ToAssetPairRaw(),
                "SWT_XRP".ToAssetPairRaw(),
                "BCH_ETH".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }
    }
}
